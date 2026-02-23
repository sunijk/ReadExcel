using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Model
{
    public class SubLoanColumnMapping
    {
        public string PrimaryVGD { get; set; } = string.Empty;
        public string IFRS9Stage { get; set; } = string.Empty;
        public string TypeOfCreditInstrument { get; set; } = string.Empty;
        public string PRSSubLimit { get; set; } = string.Empty;
        public string LoanStartDate { get; set; } = string.Empty;
        public string LoanEndDate { get; set; } = string.Empty;
    }
}
