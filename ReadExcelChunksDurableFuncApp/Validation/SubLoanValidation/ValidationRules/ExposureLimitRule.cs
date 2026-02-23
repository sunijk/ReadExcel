using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules
{
    public class ExposureLimitRule : ISubLoanRule
    {
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            //  logic
        }
    }
}
