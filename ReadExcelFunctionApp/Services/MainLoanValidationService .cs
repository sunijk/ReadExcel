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
    /// Validates the main loan header section extracted from the Excel workbook.
    /// Populates ValidationResult.HeaderErrors
    /// </summary>
    public class MainLoanValidationService : IMainLoanValidationService
    {
        private readonly ILogger<MainLoanValidationService> _logger;

        public MainLoanValidationService(ILogger<MainLoanValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Applies validation rules to the MainLoanDetails/>.
        // Any validations are appended to validation.HeaderErrors.
        /// </summary>
        /// <param name="mainLoan">The main loan header data to validate.</param>

        public void Validate(MainLoanDetails mainLoan, ValidationResult validation)
        {
            if (mainLoan == null)
                throw new ArgumentNullException(nameof(mainLoan), "MainLoanDetails cannot be null.");

            if (validation == null)
                throw new ArgumentNullException(nameof(validation), "ValidationResult cannot be null.");

            _logger.LogInformation("Starting main loan header validation.");

            try
            {

                if (string.IsNullOrWhiteSpace(mainLoan.SubLoansOriginated))
                {
                    validation.HeaderErrors.Add("Sub-loans originated is mandatory.");
                }

                if (string.IsNullOrWhiteSpace(mainLoan.SubLoansReport))
                {
                    validation.HeaderErrors.Add("Suub-loans report is mandatory.");
                }

                _logger.LogInformation("Main loan header validation complete. ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during main loan header validation.");
                throw;
            }
        }
    }
}
