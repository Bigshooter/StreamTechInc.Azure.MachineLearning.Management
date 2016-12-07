using Newtonsoft.Json;
using System;

namespace SteamTechInc.Azure.MachineLearning.Management
{
    public class WebServiceProperties
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonProperty(PropertyName = "provisioningState")]
        public ProvisioningState ProvisioningState { get; set; }

        [JsonProperty(PropertyName = "keys")]
        public WebServiceKeys Keys { get; set; }

        [JsonProperty(PropertyName = "readyOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty(PropertyName = "swaggerLocation")]
        public string SwaggerLocation { get; set; }

        [JsonProperty(PropertyName = "exposeSampleData")]
        public bool ExposeSampleData { get; set; }

        [JsonProperty(PropertyName = "realtimeConfiguration")]
        public RealtimeConfiguration RealtimeConfiguration { get; set; }
    }
}