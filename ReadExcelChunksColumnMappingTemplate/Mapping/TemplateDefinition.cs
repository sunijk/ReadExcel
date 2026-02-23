using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMappingTemplate.Mapping
{
    public class TemplateDefinition
    {
        [JsonProperty("TemplateName")]
        public string TemplateName { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Model property name → Excel column header name.
        /// e.g. "PrimaryVGD" → "Primary VGD"
        /// </summary>
        [JsonProperty("Mapping")]
        public Dictionary<string, string> Mapping { get; set; }
            = new Dictionary<string, string>();
    }
}
