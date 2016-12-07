using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class ColumnDescriptor
    {
        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "Format")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "x-ms-isnullable")]
        public bool Isnullable {get;set;}
    }
}
