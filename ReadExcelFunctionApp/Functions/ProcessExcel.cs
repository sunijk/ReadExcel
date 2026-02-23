using ReadExcelcFunctionApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace ReadExcelcFunctionApp.Functions
{
    /// <summary>
    /// Azure Function responsible for processing Excel files.
    /// Triggered via HTTP GET or POST requests.
    /// </summary>
    public class ProcessExcel
    {
        private readonly IExcelReaderService _excelService;
        private readonly ILogger<ProcessExcel> _logger;
        public ProcessExcel(IExcelReaderService excelService, ILogger<ProcessExcel> logger)
        {
            _excelService = excelService;
            _logger = logger;
        }

        /// <summary>
        /// HTTP-triggered Azure Function entry point.
        /// Reads a loan Excel file from a local path, processes and validates its contents,
        /// and returns the validation result .
        /// </summary>
        [FunctionName("ProcessExcel")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ProcessExcel")]
        HttpRequest req)
        {
            _logger.LogInformation("ProcessExcel function triggered at {Time} UTC.", DateTime.UtcNow);

            // Start time
            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            // var response = req.CreateResponse();

            // Hardcoded local file path (for testing)
            string filePath = @"C:\Users\sunij\Desktop\EBRD-POC\LoanDetails.xlsx";

            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Excel file not found at path: {FilePath}", filePath);
                    return new BadRequestErrorMessageResult("File not found.");
                }

                _logger.LogInformation("Processing Excel file: {FilePath}", filePath);

                // Process Excel & validate excel
                var result = _excelService.ReadExcel(filePath);

                stopwatch.Stop();
                var endTime = DateTime.UtcNow;

                var executionInfo = new
                {
                    StartTimeUtc = startTime,
                    EndTimeUtc = endTime,
                    TotalSeconds = stopwatch.Elapsed.TotalSeconds
                };

                var finalResponse = new
                {
                    Execution = executionInfo,
                    Data = result.Validation
                };

                return new OkObjectResult(finalResponse);
            }
            catch (Exception ex)
            {
                // Catch-all for any Errors during processing
                _logger.LogError(ex, "Error occurred while processing Excel file.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
