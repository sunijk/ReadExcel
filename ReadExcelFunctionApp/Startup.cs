using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ReadExcelcFunctionApp.Services.Interfaces;
using ReadExcelcFunctionApp.Services;
using Microsoft.Extensions.Logging;

// Attribute MUST be here
[assembly: FunctionsStartup(typeof(ReadExcelcFunctionApp.Startup))]

namespace ReadExcelcFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
            });
           
            //  Register application services
            builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
            builder.Services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
            builder.Services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
        }
    }
}