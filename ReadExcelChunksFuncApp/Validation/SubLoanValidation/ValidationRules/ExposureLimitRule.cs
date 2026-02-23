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
    /// Validates that the  ExposureLimit is required.
    /// </summary>
    public class ExposureLimitRule : ISubLoanRule
    {
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            //  logic
        }
    }
}
