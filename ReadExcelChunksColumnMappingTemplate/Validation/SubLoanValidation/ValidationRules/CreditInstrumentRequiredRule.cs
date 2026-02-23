using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules
{
    public class CreditInstrumentRequiredRule : ISubLoanRule
    {
        public string TargetProperty => "TypeOfCreditInstrument";
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(model.TypeOfCreditInstrument))
            {
                result.RecordErrors.Add(
                    $"Row {rowNumber}: Type of Credit Instrument is mandatory.");
            }
        }
    }
}
