using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using LoanReadExcelChunksFuncApp.ValidationRules;
using System.Collections.Generic;

namespace LoanReadExcelChunksFuncApp.Services
{
    public class SubLoanValidationService : ISubLoanValidationService
    {
        private readonly ISubLoanRuleFactory ruleFactory;
        private readonly SubLoanRuleEngine ruleEngine;
        public SubLoanValidationService(ISubLoanRuleFactory ruleFactory, SubLoanRuleEngine ruleEngine)
        {
            this.ruleFactory = ruleFactory;
            this.ruleEngine = ruleEngine;
        }
        public void Validate(List<SubBorrowerDetail> borrowers, ValidationResult result,
         Dictionary<string, string> columnMapping)
        {
            // Step 1: Get active rules
            var activeRules = ruleFactory.GetActiveValidationRules(columnMapping);

            // Step 2: Execute rules
            ruleEngine.Execute(borrowers, activeRules, result);
        }

        /*
                public void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation)
                {
                    for (int i = 0; i < borrowers.Count; i++)
                    {
                        var row = borrowers[i];
                        int rowNo = i + 1;

                        if (string.IsNullOrWhiteSpace(row.PrimaryVGD))
                        {
                            validation.RecordErrors.Add($"Row {rowNo}: Primary VGD is mandatory.");
                        }

                        if (string.IsNullOrWhiteSpace(row.IFRS9Stage))
                        {
                            validation.RecordErrors.Add($"Row {rowNo}: IFRS 9 Stage is mandatory.");
                        }

                        if (string.IsNullOrWhiteSpace(row.TypeOfCreditInstrument))
                        {
                            validation.RecordErrors.Add($"Row {rowNo}: Type of Credit Instrument is mandatory.");
                        }

                        if (row.PRSSubLimit <= 1000)
                        {
                            validation.RecordErrors.Add($"Row {rowNo}: PRS Sub-Limit must be greater than 1000.");
                        }

                        if (row.LoanStartDate.HasValue &&
                            row.LoanEndDate.HasValue &&
                            row.LoanStartDate >= row.LoanEndDate)
                        {
                            validation.RecordErrors.Add(
                                $"Row {rowNo}: Loan Start Date must be less than Loan End Date.");
                        }
                    }
                }
        */
    }
}
