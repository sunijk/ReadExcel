using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.Services.Interfaces;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface;
using ReadExcelChunksFuncColumnMapping.ValidationRules;
using System.Collections.Generic;

namespace ReadExcelChunksFuncColumnMapping.Services
{
    /// <summary>
    /// Validates each sub loan detail row extracted from the Excel.
    /// Populates ValidationResult.
    /// </summary>
    public class SubLoanValidationService : ISubLoanValidationService
    {
        // private readonly SubLoanRuleEngine ruleEngine;
        private readonly ISubLoanRuleFactory ruleFactory;
        public SubLoanValidationService(ISubLoanRuleFactory ruleFactory)
        {
            this.ruleFactory = ruleFactory;
        }

        /// <summary>
        /// Validates a list of sub-borrower detail records by building a rule set from the
        /// rule factory and executing each rule against every record via the rule engine.
        /// </summary>

        public void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation)
        {
            var rules = this.ruleFactory.CreateRules();

            var engine = new SubLoanRuleEngine(rules);
            engine.Execute(borrowers, validation);
        }

     
    }
}
