using ReadExcelcFunctionApp.Model;
using ReadExcelcFunctionApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelcFunctionApp.Services
{
    /// <summary>
    /// Validates each sub loan detail row extracted from the Excel workbook.
    /// Populates ValidationResult.
    /// </summary>
    public class SubLoanValidationService : ISubLoanValidationService
    {
        private readonly ILogger<SubLoanValidationService> _logger;

        public SubLoanValidationService(ILogger<SubLoanValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Iterates all sub-borrower rows and applies validation rules to each one.
        /// Any validations are appended to validation.RecordErrors.
        /// </summary>
        /// <param name="borrowers">The list of sub-borrower detail records to validate.</param>
        /// <returns>
        /// The list of record-level error messages accumulated during validation.
        /// </returns>

        public List<string> Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation)
        {
            if (borrowers == null)
                throw new ArgumentNullException(nameof(borrowers), "Borrower list cannot be null.");

            // Initialise validation result if caller did not provide one
            if (validation == null)
                validation = new ValidationResult();

            if (validation.RecordErrors == null)
                validation.RecordErrors = new List<string>();

            _logger.LogInformation("Starting sub-loan validation. Records to validate: {Count}.", borrowers.Count);

            try
            {
                for (int i = 0; i < borrowers.Count; i++)
                {
                    var row = borrowers[i];
                    int rowNo = i + 1;

                    _logger.LogDebug("Validating sub-borrower record at row {RowNo}.", rowNo);

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

                _logger.LogInformation("Sub-loan validation complete. Total record errors: {Count}.",
                  validation.RecordErrors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sub-loan validation.");
                throw;
            }
            return validation.RecordErrors;
        }
    }
}
