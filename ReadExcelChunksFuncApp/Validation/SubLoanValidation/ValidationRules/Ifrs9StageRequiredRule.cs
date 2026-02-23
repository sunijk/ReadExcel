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
    /// Validates that the  Ifrs9Stage is required.
    /// </summary>
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
