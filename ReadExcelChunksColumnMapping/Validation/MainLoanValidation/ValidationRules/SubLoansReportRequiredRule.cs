using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.ValidationRules
{
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
