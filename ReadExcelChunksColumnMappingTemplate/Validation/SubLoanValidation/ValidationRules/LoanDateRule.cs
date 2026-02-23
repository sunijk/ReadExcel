using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules
{
    public class LoanDateRule : ISubLoanRule
    {
        public string TargetProperty => "LoanDate";
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            if (model.LoanStartDate.HasValue &&
                model.LoanEndDate.HasValue &&
                model.LoanStartDate >= model.LoanEndDate)
            {
                result.RecordErrors.Add(
                    $"Row {rowNumber}: Loan Start Date must be less than Loan End Date.");
            }
        }
    }
}
