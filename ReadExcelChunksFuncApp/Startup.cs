using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ReadExcelChunksFuncApp.Services.Interfaces;
using ReadExcelChunksFuncApp.Services;
using ReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using ReadExcelChunksFuncApp.ValidationRuleEngine.ValidationRules;
using ReadExcelChunksFuncApp.ValidationRules.ValidationRules;
using ReadExcelChunksFuncApp.ValidationRules;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using ReadExcelChunksFuncApp.ValidationRules.MainLoanValidation;
using Microsoft.Extensions.Logging;

// Attribute MUST be here
[assembly: FunctionsStartup(typeof(ReadExcelChunksFuncApp.Startup))]

namespace ReadExcelChunksFuncApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // Register rules
            builder.Services.AddScoped<PrimaryVgdRequiredRule>();
            builder.Services.AddScoped<Ifrs9StageRequiredRule>();
            builder.Services.AddScoped<CreditInstrumentRequiredRule>();
            builder.Services.AddScoped<PrsSubLimitRule>();
            builder.Services.AddScoped<LoanDateRule>();

            // Register  Sub loan factory

            builder.Services.AddScoped<ISubLoanRuleFactory, SubLoanRuleFactory>();

            // Main loan factory
            builder.Services.AddScoped<IMainLoanRuleFactory, MainLoanRuleFactory>();

            // Register validation service
            builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
            builder.Services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
            builder.Services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
        }
    }
}