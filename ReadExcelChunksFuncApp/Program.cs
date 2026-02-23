//using LoanReadExcelChunksFuncApp.Model;
//using LoanReadExcelChunksFuncApp.Services;
//using LoanReadExcelChunksFuncApp.Services.Interfaces;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var host = new HostBuilder()
//    .ConfigureFunctionsWebApplication()
//    .ConfigureServices((context, services) =>
//    {
//        // Bind configuration
//        services.Configure<SubLoanColumnMapping>(
//            context.Configuration.GetSection("ColumnMappings:SubLoan"));
//        ////DI registration
//        services.AddScoped<IExcelReaderService, ExcelReaderService>();
//        services.AddScoped<IMainLoanValidationService, MainLoanValidationService>();
//        services.AddScoped<ISubLoanValidationService, SubLoanValidationService>();
//    })
//    .Build();

//host.Run();
