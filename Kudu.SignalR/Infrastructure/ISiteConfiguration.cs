﻿using Kudu.Core.Commands;
using Kudu.Core.Deployment;
using Kudu.Core.Editor;
using Kudu.Core.SourceControl;

namespace Kudu.SignalR.Infrastructure
{
    public interface ISiteConfiguration
    {
        string Name { get; }
        string ServiceUrl { get; }
        string SiteUrl { get; }

        IProjectSystem ProjectSystem { get; }
        IProjectSystem DevProjectSystem { get; }
        IDeploymentManager DeploymentManager { get; }
        IRepository Repository { get; }
        ICommandExecutor CommandExecutor { get; }
        ICommandExecutor DevCommandExecutor { get; }
    }
}