using System.Collections.Generic;

namespace ReadExcelChunksFuncColumnMapping.Mapping
{

    namespace LoanExcelFunctionApp.Mapping
    {
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
        public class ColumnMappingOptions
        {
            /// <summary>
            /// Key = Model property name
            /// Value = Excel column header
            /// </summary>
            public Dictionary<string, string> Mapping { get; set; }
                    = new Dictionary<string, string>();

        }
    }

}
