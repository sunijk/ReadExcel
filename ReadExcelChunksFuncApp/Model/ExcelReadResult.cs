using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Model
{

    public class ExcelReadResult
    {
        public MainLoanDetails MainLoan { get; set; } = new MainLoanDetails();
        public List<SubBorrowerDetail> Borrowers { get; set; } = new List<SubBorrowerDetail>();
        public ValidationResult Validation { get; set; } = new ValidationResult();
    }
}
