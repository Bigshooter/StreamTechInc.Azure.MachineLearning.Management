using Newtonsoft.Json;
using System;

namespace StreamTechInc.Azure.MachineLearning.Management
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

        [JsonProperty(PropertyName = "diagnostics")]
        public DiagnosticsConfiguration Diagnostics { get; set; }

        [JsonProperty(PropertyName = "storageAccount")]
        public StorageAccount StorageAccount { get; set; }

        [JsonProperty(PropertyName = "machineLearningWorkspace")]
        public MachineLearningWorkspace MachineLearningWorkspace { get; set; }

        [JsonProperty(PropertyName = "commitmentPlan")]
        public CommitmentPlan CommitmentPlan { get; set; }

        [JsonProperty(PropertyName = "input")]
        public ServiceInputOutputSpecification Input { get; set; }

        [JsonProperty(PropertyName = "output")]
        public ServiceInputOutputSpecification Output { get; set; }

        [JsonProperty(PropertyName = "exampleRequest")]
        public ExampleRequest ExampleRequest { get; set; }

        [JsonProperty(PropertyName = "assets")]
        public object Assets { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public object Parameters { get; set; }

        [JsonProperty(PropertyName = "packageType")]
        public object PackageType { get; set; }

    }
}