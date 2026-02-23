using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface
{
    /// <summary>
    /// Factory interface for creating main loan validation rules.
    /// </summary>
    public interface IMainLoanRuleFactory
    {
        /// <summary>
        /// Returns the list of rules to be executed for main loan validation.
        /// </summary>
        IEnumerable<IMainLoanRule> CreateRules();
    }
}
