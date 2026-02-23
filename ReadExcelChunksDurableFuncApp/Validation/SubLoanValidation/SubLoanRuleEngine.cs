using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules
{
    /// <summary>
    /// Executes all sub-loan validation rules for borrower records.
    /// </summary>
    public class SubLoanRuleEngine
    {
        private readonly IEnumerable<ISubLoanRule> rules;

        /// <summary>
        /// Initializes the rule engine with sub-loan rules.
        /// </summary>
        public SubLoanRuleEngine(IEnumerable<ISubLoanRule> rules)
        {
            this.rules = rules;
        }

        /// <summary>
        /// Executes all rules for each borrower record.
        /// </summary>
        public void Execute(List<SubBorrowerDetail> borrowers, ValidationResult result)
        {
            for (int i = 0; i < borrowers.Count; i++)
            {
                var borrower = borrowers[i];
                int rowNumber = i + 1;

                foreach (var rule in rules)
                {
                    rule.Validate(borrower, rowNumber, result);
                }
            }
        }
    }

}
