using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shipwreck.BlazorSignalR
{
    internal sealed class StartArgs
    {
        [JsonProperty(nameof(Url), DefaultValueHandling = DefaultValueHandling.Include)]
        public string Url { get; set; }

        [JsonProperty(nameof(QueryString), DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string QueryString { get; set; }

        [JsonProperty(nameof(Logging), DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Logging { get; set; }

        [JsonProperty(nameof(UseDefaultPath), DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? UseDefaultPath { get; set; }

        [JsonProperty(nameof(Hubs), DefaultValueHandling = DefaultValueHandling.Include)]
        public List<HubArgs> Hubs { get; set; }
    }
}
