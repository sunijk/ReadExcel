using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelcFunctionApp.Model
{
    public class ValidationResult
    {
        public List<string> HeaderErrors { get; set; }
        public List<string> RecordErrors { get; set; }
        public bool IsValid { get; set; }
    }
}
