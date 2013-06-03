namespace Kudu.Core.Hooks
{
    public class WebHook
    {
        public WebHook(HookEventType hookEventType, string hookAddress)
        {
            HookEventType = hookEventType;
            HookAddress = hookAddress;
        }

        public HookEventType HookEventType { get; private set; }

        public string HookAddress { get; private set; }
    }
}
