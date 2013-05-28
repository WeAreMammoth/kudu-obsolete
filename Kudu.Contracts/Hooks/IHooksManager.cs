using Kudu.Core.Deployment;

namespace Kudu.Core.Hooks
{
    public enum HookType
    {
        None,
        PostDeployment
    }

    public interface IHooksManager
    {
        void AddHook(HookType hookType, string hookAddress);

        void RemoveHook(string hookAddress);

        void PublishPostDeployment(IDeploymentStatusFile statusFile);
    }
}