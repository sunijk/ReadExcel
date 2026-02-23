using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface
{
    /// <summary>
    /// Defines a validation rule for main loan header data.
    /// Each rule contains a single validation responsibility.
    /// </summary>
    public interface IMainLoanRule
    {
        /// <summary>
        /// Validates the main loan model and adds errors to the validation result.
        /// </summary>
        void Validate(MainLoanDetails model, ValidationResult result);
    }
}
