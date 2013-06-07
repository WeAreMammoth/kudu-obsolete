﻿using Kudu.Core.Hooks;
using Newtonsoft.Json;

namespace Kudu.Services.Hooks
{
    public class SubscriptionRequest
    {
        [JsonProperty(PropertyName = "event")]
        public HookType HookType { get; set; }

        [JsonProperty(PropertyName = "target_url")]
        public string HookAddress { get; set; }
    }
}