using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Model
{
    public class SubBorrowerDetail
    {
        public string PrimaryVGD { get; set; }
        public string IFRS9Stage { get; set; }
        public string TypeOfCreditInstrument { get; set; }
        public decimal PRSSubLimit { get; set; }
        public DateTime? LoanStartDate { get; set; }
        public DateTime? LoanEndDate { get; set; }
    }
}
