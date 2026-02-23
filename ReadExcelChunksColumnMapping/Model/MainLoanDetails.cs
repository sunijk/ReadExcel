using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Model
{
    public class MainLoanDetails
    {
        public string UGB { get; set; }
        public string SubLoansOriginated { get; set; }
        public string SubLoansReport { get; set; }
        public DateTime? ReportingPeriodStart { get; set; }
        public DateTime? ReportingPeriodEnd { get; set; }
    }
}
