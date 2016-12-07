using Newtonsoft.Json;

namespace SteamTechInc.Azure.MachineLearning.Management
{
    public class WebService
    {
        [JsonProperty(PropertyName = "properties")]
        public WebServiceProperties Properties { get; set; }
    }
}
