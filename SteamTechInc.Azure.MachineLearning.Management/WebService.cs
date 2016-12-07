using Newtonsoft.Json;
using System;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class WebService
    {
        [JsonProperty(PropertyName = "properties")]
        public WebServiceProperties Properties { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public object Tags { get; set; }

        public WebService(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }
}
