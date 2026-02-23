using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Model
{

    public class ValidationResult
    {
        public List<string> HeaderErrors { get; set; } = new List<string>();
        public List<string> RecordErrors { get; set; } = new List<string>();
    }
}
