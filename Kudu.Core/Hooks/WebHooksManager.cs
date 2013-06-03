﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Kudu.Contracts.Infrastructure;
using Kudu.Contracts.Tracing;
using Kudu.Core.Deployment;
using Kudu.Core.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kudu.Core.Hooks
{
    public class WebHooksManager : IWebHooksManager
    {
        private const string HooksFileName = "hooks";

        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(30);

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly IEnvironment _environment;
        private readonly string _hooksFilePath;
        private readonly IOperationLock _hooksLock;
        private readonly IFileSystem _fileSystem;
        private readonly ITracer _tracer;

        static WebHooksManager()
        {
            JsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        public WebHooksManager(ITracer tracer, IEnvironment environment, IOperationLock hooksLock, IFileSystem fileSystem)
        {
            _tracer = tracer;
            _environment = environment;
            _hooksLock = hooksLock;
            _fileSystem = fileSystem;

            _hooksFilePath = Path.Combine(_environment.DeploymentsPath, HooksFileName);
        }

        public IEnumerable<WebHook> WebHooks
        {
            get
            {
                IEnumerable<WebHook> webHooks = null;

                _hooksLock.LockOperation(() =>
                {
                    webHooks = ReadWebHooksFromFile();
                }, LockTimeout);

                return webHooks;
            }
        }

        public void AddWebHook(HookEventType hookEventType, string hookAddress)
        {
            using (_tracer.Step("WebHooksManager.AddWebHook"))
            {
                if (hookAddress == null)
                {
                    return;
                }

                hookAddress = hookAddress.Trim();

                if (!Uri.IsWellFormedUriString(hookAddress, UriKind.RelativeOrAbsolute))
                {
                    throw new FormatException(Resources.Error_InvalidHookAddress.FormatCurrentCulture(hookAddress));
                }

                _hooksLock.LockOperation(() =>
                {
                    IList<WebHook> webHooks = ReadWebHooksFromFile();
                    if (!webHooks.Any(h => h.HookEventType == hookEventType && String.Equals(h.HookAddress, hookAddress, StringComparison.OrdinalIgnoreCase)))
                    {
                        webHooks.Add(new WebHook(hookEventType, hookAddress));
                        SaveHooksToFile(webHooks);

                        _tracer.Trace("Added web hook: type - {0}, address - {1}", hookEventType, hookAddress);
                    }
                }, LockTimeout);
            }
        }

        public void PublishPostDeployment(IDeploymentStatusFile statusFile)
        {
            var jsonObject = JsonConvert.SerializeObject(statusFile, JsonSerializerSettings);

            PublishToHooks(jsonObject, HookEventType.PostDeployment).Wait();
        }

        public void RemoveWebHook(string hookAddress)
        {
            _hooksLock.LockOperation(() =>
            {
                IList<WebHook> hooks = ReadWebHooksFromFile();
                SaveHooksToFile(hooks.Where(h => !String.Equals(h.HookAddress, hookAddress, StringComparison.OrdinalIgnoreCase)));
            }, LockTimeout);
        }

        private IEnumerable<WebHook> GetWebHooks(HookEventType hookEventType)
        {
            return WebHooks.Where(h => h.HookEventType == hookEventType);
        }

        private async Task PublishToHook(HttpClient httpClient, WebHook webHook, string jsonObject)
        {
            try
            {
                _tracer.Trace("Publish {0} to address - {1}, json - {2}", webHook.HookEventType, webHook.HookAddress, jsonObject.ToString());

                HttpResponseMessage response = await httpClient.PostAsync(webHook.HookAddress, new StringContent(jsonObject.ToString()));

                _tracer.Trace("Publish {0} to address - {1}, response - {2}", webHook.HookEventType, webHook.HookAddress, response.StatusCode);

                // Handle 410 responses by removing the web hook
                if (response.StatusCode == HttpStatusCode.Gone)
                {
                    RemoveWebHook(webHook.HookAddress);
                }
            }
            catch (Exception ex)
            {
                _tracer.Trace("Error while publishing for hook type - {0}, to address - {1}", webHook.HookEventType, webHook.HookAddress);
                _tracer.TraceError(ex);
            }
        }

        private async Task PublishToHooks(string jsonObject, HookEventType hookType)
        {
            IEnumerable<WebHook> webHooks = GetWebHooks(hookType);

            if (webHooks.Any())
            {
                using (var httpClient = new HttpClient())
                {
                    var tasks = new List<Task>();

                    foreach (var webHook in webHooks)
                    {
                        tasks.Add(PublishToHook(httpClient, webHook, jsonObject));
                    }

                    await Task.WhenAll(tasks);
                }
            }
        }

        private IList<WebHook> ReadWebHooksFromFile()
        {
            var hooks = new List<WebHook>();

            if (!_fileSystem.File.Exists(_hooksFilePath))
            {
                return hooks;
            }

            IEnumerable<string> lines = null;
            OperationManager.Attempt(() => lines = _fileSystem.File.ReadAllLines(_hooksFilePath));

            // Each line has the following format:
            // <Hook Event Type>    <Hook Address>
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (String.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                int splitIndex = trimmedLine.IndexOf('\t');
                if (splitIndex < 0)
                {
                    continue;
                }

                string hookEventTypeStr = trimmedLine.Substring(0, splitIndex);
                HookEventType hookEventType;
                bool hookTypeParsed = Enum.TryParse(hookEventTypeStr, out hookEventType);
                if (!hookTypeParsed)
                {
                    continue;
                }

                string hookAddress = trimmedLine.Substring(splitIndex + 1);
                if (!Uri.IsWellFormedUriString(hookAddress, UriKind.RelativeOrAbsolute))
                {
                    continue;
                }

                hooks.Add(new WebHook(hookEventType, hookAddress));
            }

            return hooks;
        }

        private void SaveHooksToFile(IEnumerable<WebHook> hooks)
        {
            string hooksFileContent = String.Join("\n", hooks.Select(h => h.HookEventType + "\t" + h.HookAddress));
            OperationManager.Attempt(() => _fileSystem.File.WriteAllText(_hooksFilePath, hooksFileContent));
        }
    }
}