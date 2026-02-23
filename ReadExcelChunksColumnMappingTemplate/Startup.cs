using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using LoanReadExcelChunksFuncApp.Services;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface;
using LoanReadExcelChunksFuncApp.ValidationRuleEngine.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation.Interface;
using LoanReadExcelChunksFuncApp.ValidationRules.MainLoanValidation;
using LoanReadExcelChunksFuncApp.Mapping.LoanExcelFunctionApp.Mapping;
using Microsoft.Extensions.Configuration;
using LoanReadExcelChunksFuncApp.Mapping.Interfaces;
using LoanReadExcelChunksFuncApp.Mapping;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using ReadExcelChunksFuncColumnMappingTemplate.Mapping;
using System;

// Attribute MUST be here
[assembly: FunctionsStartup(typeof(LoanReadExcelChunksFuncApp.Startup))]

namespace LoanReadExcelChunksFuncApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            // Without template


            // With multiple template
            //var options = new ColumnMappingOptions();
            //var section = config.GetSection("ColumnMapping");

            //foreach (var template in section.GetChildren())
            //{
            //    var mapping = new Dictionary<string, string>();
            //    template.Bind(mapping);

            //    options.Templates[template.Key] = mapping;
            //}

            //// Register options
            //builder.Services.AddSingleton(options);

            ColumnMappingOptions columnMappingOptions;
            try
            {
                columnMappingOptions = TemplateLoader.Load();

                Console.WriteLine($"[TemplateLoader] Folder   : {TemplateLoader.ResolvedFolderPath}");
                Console.WriteLine($"[TemplateLoader] Template : {TemplateLoader.LoadedTemplateName}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Startup failed — could not load column-mapping template.\n{ex.Message}", ex);
            }

            builder.Services.AddSingleton(columnMappingOptions);
            // Register adapter
            builder.Services.AddSingleton<IExcelRowAdapter, ConfigDrivenAdapter>();

            // Register rules
            //builder.Services.AddSingleton<PrimaryVgdRequiredRule>();
            //builder.Services.AddSingleton<Ifrs9StageRequiredRule>();
            //builder.Services.AddSingleton<CreditInstrumentRequiredRule>();
            //  builder.Services.AddSingleton<PrsSubLimitRule>();
            // builder.Services.AddSingleton<LoanDateRule>();

            builder.Services.AddSingleton<ISubLoanRule, PrimaryVgdRequiredRule>();
            builder.Services.AddSingleton<ISubLoanRule, Ifrs9StageRequiredRule>();
            builder.Services.AddSingleton<ISubLoanRule, PrsSubLimitRule>();
            builder.Services.AddSingleton<ISubLoanRule, CreditInstrumentRequiredRule>();
            builder.Services.AddSingleton<ISubLoanRule, PrsSubLimitRule>();
            builder.Services.AddSingleton<ISubLoanRule, LoanDateRule>();

            // Register  Sub loan factory

            builder.Services.AddSingleton<ISubLoanRuleFactory, SubLoanRuleFactory>();
            builder.Services.AddSingleton<SubLoanRuleEngine>();

            builder.Services.AddSingleton<ISubLoanValidationService, SubLoanValidationService>();


            // Main loan factory
            builder.Services.AddScoped<IMainLoanRuleFactory, MainLoanRuleFactory>();

            // Register validation service
            builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
            builder.Services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
            builder.Services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
        }
    }
}