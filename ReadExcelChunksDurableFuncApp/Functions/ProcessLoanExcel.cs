using LoanReadExcelChunksFuncApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace LoanReadExcelChunksFuncApp.Functions
{
    public class ProcessLoanExcel
    {
        private readonly IExcelReaderService _excelService;

        public ProcessLoanExcel(IExcelReaderService excelService)
        {
            _excelService = excelService;
        }

        [FunctionName("ProcessExcelChunks")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function
            , "get", "post", Route = "ProcessExcelChunks")]
        HttpRequest req)
        {

            // Start time
            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            //var response = req.CreateResponse();

            // Hardcoded local file path (for testing)
            string filePath = @"C:\Users\sunij\Desktop\EBRD-POC\LoanDetails.xlsx";

            if (!File.Exists(filePath))
            {
                return new BadRequestErrorMessageResult("File not found.");
            }

            // Process Excel & validate excel
            var result = _excelService.ReadExcel(filePath);

            stopwatch.Stop();
            var endTime = DateTime.UtcNow;

            var executionInfo = new
            {
                StartTimeUtc = startTime,
                EndTimeUtc = endTime,
                // TotalMilliseconds = stopwatch.ElapsedMilliseconds,
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
    }
}
