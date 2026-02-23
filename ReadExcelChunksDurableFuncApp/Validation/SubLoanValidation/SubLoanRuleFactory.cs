using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRules
{
    public class SubLoanRuleFactory : ISubLoanRuleFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SubLoanRuleFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ISubLoanRule> CreateRules()
        {
            return new List<ISubLoanRule>
        {
            _serviceProvider.GetRequiredService<PrimaryVgdRequiredRule>(),
            _serviceProvider.GetRequiredService<Ifrs9StageRequiredRule>(),
           _serviceProvider.GetRequiredService<CreditInstrumentRequiredRule>(),
            _serviceProvider.GetRequiredService<PrsSubLimitRule>(),
            _serviceProvider.GetRequiredService<LoanDateRule>()
        };
        }
    }
}

