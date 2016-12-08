using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class ColumnDescriptor
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "x-ms-isnullable")]
        public bool Isnullable {get;set;}
    }
}
