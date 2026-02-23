using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules
{
    public class Ifrs9StageRequiredRule : ISubLoanRule
    {
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(model.IFRS9Stage))
            {
                result.RecordErrors.Add(
                    $"Row {rowNumber}: IFRS 9 Stage is mandatory.");
            }
        }
    }
}
