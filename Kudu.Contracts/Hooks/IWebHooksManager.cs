using System.Collections.Generic;
using Kudu.Core.Deployment;

namespace Kudu.Core.Hooks
{
    public interface IWebHooksManager
    {
        void AddWebHook(HookEventType hookType, string hookAddress);

        void RemoveWebHook(string hookAddress);

        IEnumerable<WebHook> WebHooks { get; }

        void PublishPostDeployment(IDeploymentStatusFile statusFile);
    }
}
