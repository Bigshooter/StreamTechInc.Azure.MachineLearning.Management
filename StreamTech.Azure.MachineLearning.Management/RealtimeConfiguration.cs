using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class RealtimeConfiguration
    {
        [JsonProperty(PropertyName = "maxConcurrentCalls")]
        public int MaxConcurrentCalls { get; set; }
    }
}