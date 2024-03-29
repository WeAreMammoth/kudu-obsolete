﻿using System;
using System.IO;
using Kudu.Contracts.Settings;
using Kudu.Contracts.Tracing;
using Kudu.Core.Infrastructure;

namespace Kudu.Core.Deployment.Generator
{
    internal class ExternalCommandFactory
    {
        // TODO: Once CustomBuilder is removed, change all internals back to privates
        internal const string StarterScriptName = "starter.cmd";

        private IEnvironment _environment;
        private IDeploymentSettingsManager _deploymentSettings;
        private string _repositoryPath;

        public ExternalCommandFactory(IEnvironment environment, IDeploymentSettingsManager settings, string repositoryPath)
        {
            _environment = environment;
            _deploymentSettings = settings;
            _repositoryPath = repositoryPath;
        }

        public Executable BuildExternalCommandExecutable(string workingDirectory, string deploymentTargetPath, ILogger logger)
        {
            string sourcePath = _repositoryPath;
            string targetPath = deploymentTargetPath;

            // Creates an executable pointing to cmd and the working directory being
            // the repository root
            var exe = new Executable(StarterScriptPath, workingDirectory, _deploymentSettings.GetCommandIdleTimeout());
            exe.AddDeploymentSettingsAsEnvironmentVariables(_deploymentSettings);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.SourcePath, sourcePath, logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.TargetPath, targetPath, logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.WebRootPath, _environment.WebRootPath, logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.MSBuildPath, PathUtility.ResolveMSBuildPath(), logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.KuduSyncCommandKey, KuduSyncCommand, logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.SelectNodeVersionCommandKey, SelectNodeVersionCommand, logger);
            UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.NpmJsPathKey, PathUtility.ResolveNpmJsPath(), logger);

            if (PathUtility.PathsEquals(sourcePath, targetPath))
            {
                UpdateToDefaultIfNotSet(exe, WellKnownEnvironmentVariables.InPlaceDeployment, "1", logger);
            }

            // Disable this for now
            // exe.EnvironmentVariables[NuGetCachePathKey] = Environment.NuGetCachePath;

            // NuGet.exe 1.8 will require an environment variable to make package restore work
            exe.EnvironmentVariables[WellKnownEnvironmentVariables.NuGetPackageRestoreKey] = "true";

            exe.SetHomePath(_environment.SiteRootPath);

            // Set the path so we can add more variables
            exe.EnvironmentVariables["PATH"] = System.Environment.GetEnvironmentVariable("PATH");

            // Add the msbuild path and git path to the %PATH% so more tools are available
            var toolsPaths = new[] {
                Path.GetDirectoryName(PathUtility.ResolveMSBuildPath()),
                Path.GetDirectoryName(PathUtility.ResolveGitPath()),
                Path.GetDirectoryName(PathUtility.ResolveVsTestPath())
            };

            exe.AddToPath(toolsPaths);

            return exe;
        }

        private string KuduSyncCommand
        {
            get
            {
                return Path.Combine(_environment.ScriptPath, "kudusync");
            }
        }

        private string SelectNodeVersionCommand
        {
            get
            {
                return "node \"" + Path.Combine(_environment.ScriptPath, "selectNodeVersion") + "\"";
            }
        }

        private string StarterScriptPath
        {
            get
            {
                return Path.Combine(_environment.ScriptPath, StarterScriptName);
            }
        }

        private void UpdateToDefaultIfNotSet(Executable exe, string key, string defaultValue, ILogger logger)
        {
            var value = _deploymentSettings.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                exe.EnvironmentVariables[key] = defaultValue;
            }
            else
            {
                logger.Log("Using custom deployment setting for {0} custom value is '{1}'.", key, value);
                exe.EnvironmentVariables[key] = value;
            }
        }
    }
}
