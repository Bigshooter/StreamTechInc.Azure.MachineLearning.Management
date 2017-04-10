using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class ServiceInputOutputSpecification
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public object Properties { get; set; }

        public ServiceInputOutputSpecification()
        {
            Type = "object";
        }
    }
}