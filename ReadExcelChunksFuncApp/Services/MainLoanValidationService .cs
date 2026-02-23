using Microsoft.Extensions.Logging;
using ReadExcelChunksFuncApp.Model;
using ReadExcelChunksFuncApp.Services.Interfaces;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Services
{
    /// <summary>
    /// Handles validation of main  data using rule engine.
    ///  /// Populates ValidationResult.HeaderErrors
    /// </summary>
    public class MainLoanValidationService : IMainLoanValidationService
    {
        private readonly IMainLoanRuleFactory ruleFactory;
        private readonly ILogger<MainLoanValidationService> _logger;
        public MainLoanValidationService(IMainLoanRuleFactory ruleFactory,
            ILogger<MainLoanValidationService> logger)
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
    }
}
