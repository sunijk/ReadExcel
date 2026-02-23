using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace LoanReadExcelChunksFuncApp2.Functions
{
    public  class MyFunctions
    {
        [Function("StartHello")]
        public  async Task<HttpResponseData> StartHello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync("HelloOrchestrator");
            return client.CreateCheckStatusResponse(req, instanceId);
        }

        [Function("HelloOrchestrator")]
        public  async Task<string> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            return await context.CallActivityAsync<string>("SayHello", "World");
        }

        [Function("SayHello")]
        public  string SayHello([ActivityTrigger] string name)
        {
            return $"Hello {name}!";
        }
    }
}