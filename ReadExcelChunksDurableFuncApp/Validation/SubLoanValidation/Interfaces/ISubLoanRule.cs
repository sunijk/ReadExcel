using LoanReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface
{
    /// <summary>
    /// Defines a validation rule for sub-loan borrower records.
    /// Each rule validates a single condition on a borrower record.
    /// </summary>
    public interface ISubLoanRule
    {
        /// <summary>
        /// Validates a borrower record and appends any validation errors.
        /// </summary>
        void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result);
    }
}
