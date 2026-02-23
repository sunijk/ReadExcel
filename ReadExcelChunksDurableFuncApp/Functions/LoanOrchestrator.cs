using LoanReadExcelChunksFuncApp.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadExcelChunksDurableFuncApp.Functions
{
    public class LoanOrchestrator
    {
        [Function("LoanOrchestrator")]
        public async Task<ValidationResult> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            // 1. Receive the file path that StartLoanProcessing passed in
            string filePath = context.GetInput<string>();

            // 2. Split the Excel file into row chunks
            List<ChunkInfo> chunks = await context.CallActivityAsync<List<ChunkInfo>>(
                "SplitExcelActivity", filePath);

            // 3. Fan out — validate every chunk in parallel
            var parallelTasks = new List<Task<ValidationResult>>();
            foreach (ChunkInfo chunk in chunks)
            {
                Task<ValidationResult> task = context.CallActivityAsync<ValidationResult>(
                    "ProcessChunkActivity", chunk);

                parallelTasks.Add(task);
            }

            // Wait for all chunks to finish
            ValidationResult[] chunkResults = await Task.WhenAll(parallelTasks);

            // 4. Merge all chunk ValidationResults into one
            var merged = new ValidationResult();
            foreach (ValidationResult chunkResult in chunkResults)
            {
                merged.HeaderErrors.AddRange(chunkResult.HeaderErrors);
                merged.RecordErrors.AddRange(chunkResult.RecordErrors);
            }

            // 5. Return the merged result 
            return merged;
        }
    }
}

//using LoanReadExcelChunksFuncApp.Model;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.DurableTask;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace ReadExcelChunksDurableFuncApp.Functions
//{
//    public class LoanOrchestrator
//    {
//        [Function("LoanOrchestrator")]
//        public async Task<IActionResult> RunOrchestrator(
//            [OrchestrationTrigger] TaskOrchestrationContext context)
//        {
//            // Step 1: Split into chunks
//            var chunks = await context.CallActivityAsync<List<ChunkInfo>>(
//                "SplitExcelActivity");

//            // Step 2: Fan-out (parallel processing)
//            var tasks = new List<Task<ValidationResult>>();

//            foreach (var chunk in chunks)
//            {
//                tasks.Add(context.CallActivityAsync<ValidationResult>(
//                    "ProcessChunkActivity",
//                    chunk));
//            }

//            // Step 3: Fan-in (wait for all)
//            var results = await Task.WhenAll(tasks);

//            // Step 4: Merge results
//            var finalResult = new ValidationResult();

//            foreach (var result in results)
//            {
//                finalResult.HeaderErrors.AddRange(result.HeaderErrors);
//                finalResult.RecordErrors.AddRange(result.RecordErrors);
//            }

//            return new OkObjectResult(finalResult);
//        }
//    }

//}
