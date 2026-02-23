using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.ValidationRules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation
{
    /// <summary>
    /// Default implementation of the main loan rule factory.
    /// Responsible for assembling all main loan validation rules.
    /// </summary>
    public class MainLoanRuleFactory : IMainLoanRuleFactory
    {
        private readonly IServiceProvider serviceProvider;

        public MainLoanRuleFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates and returns the configured main loan rules.
        /// </summary>
        public IEnumerable<IMainLoanRule> CreateRules()
        {
            return new List<IMainLoanRule>
        {
            serviceProvider.GetRequiredService<SubLoansOriginatedRequiredRule>(),
            serviceProvider.GetRequiredService<SubLoansReportRequiredRule>(),
            serviceProvider.GetRequiredService<ReportingPeriodDateRule>()
        };
        }
    }

}
