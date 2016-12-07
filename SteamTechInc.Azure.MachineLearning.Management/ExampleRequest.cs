using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class ExampleRequest
    {

        [JsonProperty(PropertyName = "inputs")]
        public object Inputs { get; set; }

        [JsonProperty(PropertyName = "globalParameters")]
        public object GlobalParameters { get; set; }
    }
}