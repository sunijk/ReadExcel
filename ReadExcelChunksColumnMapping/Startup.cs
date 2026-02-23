using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ReadExcelChunksFuncColumnMapping.Services.Interfaces;
using ReadExcelChunksFuncColumnMapping.Services;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.ValidationRules;
using ReadExcelChunksFuncColumnMapping.ValidationRules.ValidationRules;
using ReadExcelChunksFuncColumnMapping.ValidationRules;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation;
using ReadExcelChunksFuncColumnMapping.Mapping.LoanExcelFunctionApp.Mapping;
using Microsoft.Extensions.Configuration;
using ReadExcelChunksFuncColumnMapping.Mapping.Interfaces;
using ReadExcelChunksFuncColumnMapping.Mapping;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

// Attribute MUST be here
[assembly: FunctionsStartup(typeof(ReadExcelChunksFuncColumnMapping.Startup))]

namespace ReadExcelChunksFuncColumnMapping
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Retrieve configuration localsettings.json
            var config = builder.GetContext().Configuration;

            // Bind the "ColumnMapping" section to options object.
            // This gives compile-time safety when accessing individual column name properties
            var options = new ColumnMappingOptions();
            config.GetSection("ColumnMapping").Bind(options);

            var mapping = new Dictionary<string, string>();

            config.GetSection("ColumnMapping").Bind(mapping);

            builder.Services.AddSingleton<IExcelRowAdapter>(
                new ConfigDrivenAdapter(mapping));


            //// Register options
            builder.Services.AddSingleton(options);
            //// Register options
            builder.Services.AddSingleton(mapping);

            // Register adapter
            builder.Services.AddSingleton<IExcelRowAdapter, ConfigDrivenAdapter>();

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