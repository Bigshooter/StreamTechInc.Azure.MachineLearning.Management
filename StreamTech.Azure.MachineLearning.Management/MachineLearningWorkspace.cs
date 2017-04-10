using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class MachineLearningWorkspace
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}