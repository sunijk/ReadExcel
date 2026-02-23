using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoanReadExcelChunksFuncApp.ValidationRules
{
    public class SubLoanRuleFactory : ISubLoanRuleFactory
    {
        private readonly IEnumerable<ISubLoanRule> allRules;

        public SubLoanRuleFactory(IEnumerable<ISubLoanRule> allRules)
        {
            this.allRules = allRules;
        }

        /// <summary>
        /// Returns only the rules whose TargetProperty exists as a key
        /// in the active template's Mapping dictionary.
        ///
        /// e.g. Template2 has no "TypeOfCreditInstrument" key so
        /// CreditInstrumentRequiredRule is excluded automatically.
        /// </summary>
        public List<ISubLoanRule> GetActiveValidationRules(
            Dictionary<string, string> columnMapping)
        {
            var activeProperties = new HashSet<string>(
                columnMapping.Keys, StringComparer.OrdinalIgnoreCase);

#if DEBUG
            // Uncomment to diagnose rule filtering in the Output/Debug window
            foreach (var key in columnMapping.Keys)
                System.Diagnostics.Debug.WriteLine($"[Template key]     {key}");

            foreach (var rule in allRules)
                System.Diagnostics.Debug.WriteLine($"[Rule target]      {rule.TargetProperty}");
#endif

            var rules = allRules
                .Where(r => activeProperties.Contains(r.TargetProperty))
                .ToList();
            return allRules
                .Where(r => activeProperties.Contains(r.TargetProperty))
                .ToList();
        }
    }
}