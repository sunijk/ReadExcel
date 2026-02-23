using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ReadExcelChunksFuncColumnMapping.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace ReadExcelChunksFuncColumnMapping.Functions
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

        [FunctionName("Chunks")]
        public async Task<IActionResult> Run(
       [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ProcessExcelChunks")]
        HttpRequest req)
        {
            _logger.LogInformation("ProcessExcel function triggered at {Time} UTC.", DateTime.UtcNow);

            // Start time
            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();

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
                    TotalMilliseconds = stopwatch.ElapsedMilliseconds,
                    TotalSeconds = stopwatch.Elapsed.TotalSeconds
                };

                var finalResponse = new
                {
                    Execution = executionInfo,
                    Message = "Read Excel in chunks",
                    Data = result.Validation
                };

                return new OkObjectResult(finalResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Excel file.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
