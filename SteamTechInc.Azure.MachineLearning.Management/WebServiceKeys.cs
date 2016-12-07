using Newtonsoft.Json;

namespace SteamTechInc.Azure.MachineLearning.Management
{
    public class WebServiceKeys
    {
        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "secondary")]
        public string Secondary { get; set; }

    }
}