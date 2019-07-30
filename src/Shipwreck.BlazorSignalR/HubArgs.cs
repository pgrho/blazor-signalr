using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shipwreck.BlazorSignalR
{
    internal sealed class HubArgs
    {
        [JsonProperty(nameof(Name), DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; }

        [JsonProperty(nameof(Callbacks), DefaultValueHandling = DefaultValueHandling.Include)]
        public List<CallbackArgs> Callbacks { get; set; }
    }
}
