using ReadExcelChunksFuncApp.Model;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.ValidationRules
{
    /// <summary>
    /// Validates that the SubLoansReport is required.
    /// </summary>
    public class SubLoansReportRequiredRule : IMainLoanRule
    {
        public void Validate(MainLoanDetails model, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(model.SubLoansReport))
            {
                result.HeaderErrors.Add("Sub-loans report is mandatory.");
            }
        }
    }
}
