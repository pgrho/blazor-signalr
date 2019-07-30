using Newtonsoft.Json;

namespace Shipwreck.BlazorSignalR
{
    internal sealed class CallbackArgs
    {
        [JsonProperty(nameof(Name), DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; }

        [JsonProperty(nameof(Length), DefaultValueHandling = DefaultValueHandling.Include)]
        public int Length { get; set; }
    }
}
