using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation
{
    /// Executes all registered main loan validation rules.
    /// </summary>
    public class MainLoanRuleEngine
    {
        private readonly IEnumerable<IMainLoanRule> rules;

        /// <summary>
        /// Initializes the rule engine with a collection of rules.
        /// </summary>
        public MainLoanRuleEngine(IEnumerable<IMainLoanRule> rules)
        {
            this.rules = rules;
        }

        /// <summary>
        /// Executes each rule against the main loan model.
        /// </summary>
        public void Execute(MainLoanDetails model, ValidationResult result)
        {
            foreach (var rule in rules)
            {
                rule.Validate(model, result);
            }
        }
    }
}
