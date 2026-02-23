using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.ValidationRules;
using ReadExcelChunksFuncColumnMapping.ValidationRules.ValidationRules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules
{
    /// <summary>
    /// Factory responsible for assembling the complete set of sub-loan validation rules.
    /// Implements the Factory pattern so that rule creation is centralised in one place —
    /// adding, removing, or reordering rules only requires a change here.
    /// </summary>
    public class SubLoanRuleFactory : ISubLoanRuleFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SubLoanRuleFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates and returns the ordered list of sub-loan validation rules to be
        /// executed by the rule engine.
        /// </summary>
        /// <remarks>
        /// Rules are resolved from the DI container via 
        /// ServiceProviderServiceExtensions.GetRequiredService{T}" so that
        /// each rule's own dependencies are satisfied automatically.
        /// </remarks>
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

