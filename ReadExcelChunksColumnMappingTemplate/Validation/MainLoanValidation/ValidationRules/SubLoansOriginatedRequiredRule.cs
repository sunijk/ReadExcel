using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.ValidationRules
{
    public class SubLoansOriginatedRequiredRule : IMainLoanRule
    {
        public void Validate(MainLoanDetails model, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(model.SubLoansOriginated))
            {
                result.HeaderErrors.Add("Sub-loans originated is mandatory.");
            }
        }
    }
}
