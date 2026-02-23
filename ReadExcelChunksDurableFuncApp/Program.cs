using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.DurableTask;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules;
using LoanReadExcelChunksFuncApp.Services;
using Microsoft.Extensions.DependencyInjection;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using LoanReadExcelChunksFuncApp.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation; // Required


namespace LoanReadExcelChunksFuncApp2
{

    public class Program
    {
        public static void Main()
        {
            // Essential for .NET 4.8 Isolated
            Microsoft.Azure.Functions.Worker.FunctionsDebugger.Enable();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                 .ConfigureServices(services =>
            {
                // Register your service here
                services.AddScoped<IExcelReaderService, ExcelReaderService>();
                //// Register rules
                services.AddScoped<PrimaryVgdRequiredRule>();
                services.AddScoped<Ifrs9StageRequiredRule>();
                services.AddScoped<CreditInstrumentRequiredRule>();
                services.AddScoped<PrsSubLimitRule>();
                services.AddScoped<LoanDateRule>();

                //// Register  Sub loan factory

                services.AddScoped<ISubLoanRuleFactory, SubLoanRuleFactory>();

                //// Main loan factory
                services.AddScoped<IMainLoanRuleFactory, MainLoanRuleFactory>();

                //// Register validation service
                services.AddScoped<IExcelReaderService, ExcelReaderService>();
                services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
                services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
            })
            .Build();

            host.Run();
        }
    }
}
