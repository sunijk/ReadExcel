using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRuleEngine.ValidationRules
{
    public class PrsSubLimitRule : ISubLoanRule
    {
        public string TargetProperty => "PrsSubLimit";
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            if (model.PRSSubLimit <= 1000)
            {
                result.RecordErrors.Add($"Row {rowNumber}: PRS Sub-Limit must be greater than 1000.");
            }
        }
    }
}
