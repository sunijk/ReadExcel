using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Services
{
    /// <summary>
    /// Handles validation of main loan data using rule engine.
    /// </summary>
    public class MainLoanValidationService : IMainLoanValidationService
    {
        private readonly IMainLoanRuleFactory ruleFactory;
        public MainLoanValidationService(IMainLoanRuleFactory ruleFactory)
        {
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
