using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.ValidationRules
{
    public class PrimaryVgdRequiredRule : ISubLoanRule
    {
        /// <summary>
        /// Validates that the Primary VGD field is provided for each sub-loan record.
        /// </summary>
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(model.PrimaryVGD))
            {
                result.RecordErrors.Add($"Row {rowNumber}: Primary VGD is mandatory.");
            }
        }
    }
}
