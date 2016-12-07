using Newtonsoft.Json;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class CommitmentPlan
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}