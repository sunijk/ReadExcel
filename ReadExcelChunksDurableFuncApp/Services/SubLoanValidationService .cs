using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using LoanReadExcelChunksFuncApp.ValidationRules;
using System.Collections.Generic;

namespace LoanReadExcelChunksFuncApp.Services
{
    public class SubLoanValidationService : ISubLoanValidationService
    {
        // private readonly SubLoanRuleEngine ruleEngine;
        private readonly ISubLoanRuleFactory ruleFactory;
        public SubLoanValidationService(ISubLoanRuleFactory ruleFactory)
        {
            this.ruleFactory = ruleFactory;
        }
        public void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation)
        {
            var rules = this.ruleFactory.CreateRules();

            var engine = new SubLoanRuleEngine(rules);
            engine.Execute(borrowers, validation);
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
