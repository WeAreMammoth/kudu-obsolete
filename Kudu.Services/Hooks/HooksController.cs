using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kudu.Contracts.Tracing;
using Kudu.Core.Hooks;

namespace Kudu.Services.Hooks
{
    public class HooksController : ApiController
    {
        private readonly ITracer _tracer;
        private readonly HooksManager _hooksManager;

        public HooksController(ITracer tracer, HooksManager hooksManager)
        {
            _tracer = tracer;
            _hooksManager = hooksManager;
        }

        [HttpPost]
        public HttpResponseMessage Subscribe(SubscriptionRequest subscriptionRequest)
        {
            _hooksManager.AddHook(subscriptionRequest.HookType, subscriptionRequest.HookAddress);
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        }

        [HttpPost]
        public HttpResponseMessage Unsubscribe(SubscriptionRequest subscriptionRequest)
        {
            _hooksManager.RemoveHook(subscriptionRequest.HookAddress);
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
    }
}