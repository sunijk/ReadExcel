using ReadExcelChunksFuncColumnMappingTemplate.Mapping;
using System.Collections.Generic;

namespace LoanReadExcelChunksFuncApp.Mapping
{

    namespace LoanExcelFunctionApp.Mapping
    {

        /// <summary>
        /// Aggregates all loaded templates.
        /// Populated by <see cref="TemplateLoader"/> at startup.
        /// </summary>
        public class ColumnMappingOptions
        {
            /// <summary>TemplateName → TemplateDefinition</summary>
            public Dictionary<string, TemplateDefinition> Templates { get; set; }
                = new Dictionary<string, TemplateDefinition>();
        }
    }

}

/// <summary>
/// Holds the mapping between model properties and Excel column names.
/// Loaded from configuration.
/// 
/// Example:
/// {
///   "PrimaryVGD": "Primary VGD",
///   "IFRS9Stage": "IFRS 9 Stage"
/// }
/// </summary>
//public class ColumnMappingOptions
//{
//    /// <summary>
//    /// Key = Model property name
//    /// Value = Excel column header
//    /// </summary>
//    //public Dictionary<string, string> Mapping { get; set; }
//    //    = new Dictionary<string, string>();

//    // TemplateName -> PropertyMapping
//    public Dictionary<string, Dictionary<string, string>> Templates
//        = new Dictionary<string, Dictionary<string, string>>();
//}

