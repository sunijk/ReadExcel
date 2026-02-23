using Microsoft.Azure.Functions.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection;
//using LoanReadExcelChunksFuncApp2.Services.Interfaces;
//using LoanReadExcelChunksFuncApp2.Services;
//using LoanReadExcelChunksFuncApp2.ValidationRuleEngine.Interface;
//using LoanReadExcelChunksFuncApp2.ValidationRuleEngine.ValidationRules;
//using LoanReadExcelChunksFuncApp2.ValidationRules.ValidationRules;
//using LoanReadExcelChunksFuncApp2.ValidationRules;
//using LoanReadExcelChunksFuncApp2.ValidationRules.MainLoanValidation.Interface;
//using LoanReadExcelChunksFuncApp2.ValidationRules.MainLoanValidation;

// Attribute MUST be here
[assembly: FunctionsStartup(typeof(LoanReadExcelChunksFuncApp2.Startup))]

namespace LoanReadExcelChunksFuncApp2
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //// Register rules
            //builder.Services.AddScoped<PrimaryVgdRequiredRule>();
            //builder.Services.AddScoped<Ifrs9StageRequiredRule>();
            //builder.Services.AddScoped<CreditInstrumentRequiredRule>();
            //builder.Services.AddScoped<PrsSubLimitRule>();
            //builder.Services.AddScoped<LoanDateRule>();

            //// Register  Sub loan factory

            //builder.Services.AddScoped<ISubLoanRuleFactory, SubLoanRuleFactory>();

            //// Main loan factory
            //builder.Services.AddScoped<IMainLoanRuleFactory, MainLoanRuleFactory>();

            //// Register validation service
            //builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
            //builder.Services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
            //builder.Services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
        }
    }
}