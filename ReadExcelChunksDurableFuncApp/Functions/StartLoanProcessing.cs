using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ReadExcelChunksDurableFuncApp.Functions
{
    public class StartLoanProcessing
    {
        private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(230);


        [Function("StartProcessing")]
        public async Task<HttpResponseData> StartProcessing(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("LoanOrchestrator");

            using var cts = new CancellationTokenSource(HttpTimeout);

            OrchestrationMetadata metadata = await client.WaitForInstanceCompletionAsync(
               instanceId,
               getInputsAndOutputs: true,          // needed to read the output
               cancellation: cts.Token);

            //  Orchestration finished within the timeout — return the result
            if (metadata.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
            {
                ValidationResult validationResult =
                    metadata.ReadOutputAs<ValidationResult>();

                var okResponse = req.CreateResponse(HttpStatusCode.OK);
                await okResponse.WriteAsJsonAsync(new
                {
                    InstanceId = instanceId,
                    //IsValid = validationResult.IsValid,
                    Validation = validationResult
                });
                return okResponse;
            }

            //  Orchestration failed
            if (metadata.RuntimeStatus == OrchestrationRuntimeStatus.Failed)
            {
                var failResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await failResponse.WriteAsJsonAsync(new
                {
                    InstanceId = instanceId,
                    Error = "Orchestration failed.",
                    Details = metadata.FailureDetails?.ErrorMessage
                });
                return failResponse;
            }

            //  Still running (timeout hit) — return 202 with polling URLs
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
