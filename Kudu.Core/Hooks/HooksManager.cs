using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kudu.Contracts.Tracing;
using Kudu.Core.Deployment;
using Kudu.Core.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kudu.Core.Hooks
{
    public class HooksManager : IHooksManager
    {
        private const string HooksFileName = "hooks";

        private ITracer _tracer;
        private IEnvironment _environment;
        private string _hooksFilePath;

        public HooksManager(ITracer tracer, IEnvironment environment)
        {
            _tracer = tracer;
            _environment = environment;
            _hooksFilePath = Path.Combine(_environment.DeploymentsPath, HooksFileName);
        }

        public void AddHook(HookType hookType, string hookAddress)
        {
            if (String.IsNullOrEmpty(hookAddress))
            {
                return;
            }

            hookAddress = hookAddress.Trim();

            IList<Tuple<HookType, string>> hooks = ReadHooksFromFile();
            if (!hooks.Any(h => h.Item1 == hookType && String.Equals(h.Item2, hookAddress, StringComparison.OrdinalIgnoreCase)))
            {
                hooks.Add(new Tuple<HookType, string>(hookType, hookAddress));
                _tracer.Trace("Added hook: type - {0}, address - {1}", hookType, hookAddress);
                SaveHooksToFile(hooks);
            }
        }

        private void SaveHooksToFile(IEnumerable<Tuple<HookType, string>> hooks)
        {
            string hooksFileContent = String.Join("\n", hooks.Select(h => h.Item1 + "\t" + h.Item2));
            OperationManager.Attempt(() => File.WriteAllText(_hooksFilePath, hooksFileContent));
        }

        private IList<Tuple<HookType, string>> ReadHooksFromFile()
        {
            List<Tuple<HookType, string>> hooks = new List<Tuple<HookType, string>>();

            if (!File.Exists(_hooksFilePath))
            {
                return hooks;
            }

            IEnumerable<string> lines = null;
            OperationManager.Attempt(() => lines = File.ReadLines(_hooksFilePath));

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (String.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                int splitIndex = trimmedLine.IndexOf('\t');
                string hookTypeStr = trimmedLine.Substring(0, splitIndex);
                HookType hookType;
                bool hookTypeParsed = Enum.TryParse(hookTypeStr, out hookType);
                if (!hookTypeParsed)
                {
                    continue;
                }
                string hookAddress = trimmedLine.Substring(splitIndex + 1);

                hooks.Add(new Tuple<HookType, string>(hookType, hookAddress));
            }

            return hooks;
        }

        public void RemoveHook(string hookAddress)
        {
            IList<Tuple<HookType, string>> hooks = ReadHooksFromFile();
            SaveHooksToFile(hooks.Where(h => !String.Equals(h.Item2, hookAddress, StringComparison.OrdinalIgnoreCase)));
        }

        public void PublishPostDeployment(IDeploymentStatusFile statusFile)
        {
            var jsonSerialzerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            jsonSerialzerSettings.Converters.Add(new StringEnumConverter());
            var jsonObject = JsonConvert.SerializeObject(statusFile, jsonSerialzerSettings);

            PublishToHooks(jsonObject, HookType.PostDeployment).Wait();
        }

        private async Task PublishToHooks(string jsonObject, HookType hookType)
        {
            using (var httpClient = new HttpClient())
            {
                foreach (var hookAddress in GetHookAddresses(hookType))
                {
                    try
                    {
                        // TODO: handle 410 by removing the address
                        _tracer.Trace("Publish {0} to address - {1}, json - {2}", hookType, hookAddress, jsonObject.ToString());
                        await httpClient.PostAsync(hookAddress, new StringContent(jsonObject.ToString()));
                    }
                    catch (Exception ex)
                    {
                        _tracer.Trace("Error while publishing for hook type - {0}, to address - {1}", hookType, hookAddress);
                        _tracer.TraceError(ex);
                    }
                }
            }
        }

        public IEnumerable<string> GetHookAddresses(HookType hookType)
        {
            IList<Tuple<HookType, string>> hooks = ReadHooksFromFile();
            return hooks.Where(h => h.Item1 == hookType).Select(h => h.Item2);
        }
    }
}