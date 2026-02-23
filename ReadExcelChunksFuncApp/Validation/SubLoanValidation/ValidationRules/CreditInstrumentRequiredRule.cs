using ReadExcelChunksFuncApp.Model;
using ReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.ValidationRules.ValidationRules
{
    /// <summary>
    /// Validates that the  CreditInstrument is required.
    /// </summary>
    public class CreditInstrumentRequiredRule : ISubLoanRule
    {
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
