using Microsoft.Extensions.Logging;
using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.Services.Interfaces;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Services
{
    /// <summary>
    /// Handles validation of main loan data using rule engine.
    /// </summary>
    public class MainLoanValidationService : IMainLoanValidationService
    {
        private readonly IMainLoanRuleFactory ruleFactory;
        private readonly ILogger<MainLoanValidationService> _logger;
        public MainLoanValidationService(IMainLoanRuleFactory ruleFactory, ILogger<MainLoanValidationService> logger)
        {
            _logger = logger;
            this.ruleFactory = ruleFactory;
        }

        /// <summary>
        /// Executes all main loan validation rules.
        /// </summary>
        public void Validate(MainLoanDetails mainLoan, ValidationResult validation)
        {
            var rules = ruleFactory.CreateRules();
            var engine = new MainLoanRuleEngine(rules);
            engine.Execute(mainLoan, validation);
        }

        /* validation rules hard coded
        public void Validate(MainLoanDetails mainLoan, ValidationResult validation)
        {
            if (string.IsNullOrWhiteSpace(mainLoan.SubLoansOriginated))
            {
                validation.HeaderErrors.Add("Sub-loans originated is mandatory.");
            }

            if (string.IsNullOrWhiteSpace(mainLoan.SubLoansReport))
            {
                validation.HeaderErrors.Add("Suub-loans report is mandatory.");
            }
        }
        */
    }
}
