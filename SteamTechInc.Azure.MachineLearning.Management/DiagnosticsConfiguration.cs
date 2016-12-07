using Newtonsoft.Json;
using System;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class DiagnosticsConfiguration
    {
        [JsonProperty(PropertyName = "level")]
        public DiagnosticsLevel Level { get; set; }
        [JsonProperty(PropertyName = "expiry")]
        public DateTime Expiry { get; set; }
    }
}