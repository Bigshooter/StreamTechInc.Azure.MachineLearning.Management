using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class StorageAccount
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
    }
}