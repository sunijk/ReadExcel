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
        private readonly IEnumerable<ISubLoanRule> allRules;

        public SubLoanRuleFactory(IEnumerable<ISubLoanRule> allRules)
        {
            this.allRules = allRules;
        }

        public List<ISubLoanRule> GetActiveValidationRules(Dictionary<string, string> columnMapping)
        {
            var activeProperties = new HashSet<string>(columnMapping.Keys,
                StringComparer.OrdinalIgnoreCase);
            var rules= allRules
                .Where(r => activeProperties.Contains(r.TargetProperty))
                .ToList();

            //foreach (var key in columnMapping.Keys)
            //{
            //    System.Diagnostics.Debug.WriteLine("Mapping key: " + key);
            //}

            //foreach (var rule in allRules)
            //{
            //    System.Diagnostics.Debug.WriteLine("Rule TargetProperty: " + rule.TargetProperty);
            //}

            return allRules
                .Where(r => activeProperties.Contains(r.TargetProperty))
                .ToList();
        }
    }
}

//public SubLoanRuleFactory(IServiceProvider serviceProvider)
//{
//    _serviceProvider = serviceProvider;
//}

//public IEnumerable<ISubLoanRule> CreateRules()
//{
//    return new List<ISubLoanRule>
//{
//    _serviceProvider.GetRequiredService<PrimaryVgdRequiredRule>(),
//    _serviceProvider.GetRequiredService<Ifrs9StageRequiredRule>(),
//   _serviceProvider.GetRequiredService<CreditInstrumentRequiredRule>(),
//    _serviceProvider.GetRequiredService<PrsSubLimitRule>(),
//    _serviceProvider.GetRequiredService<LoanDateRule>()
//};
//}


