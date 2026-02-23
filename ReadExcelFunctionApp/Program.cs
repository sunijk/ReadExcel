//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using LoanExcelFunctionApp.Services.Interfaces;
//using LoanExcelFunctionApp.Services;

//public class Program
//{
//    public static void Main()
//    {
//        var host = new HostBuilder()
//            .ConfigureFunctionsWorkerDefaults()
//            .ConfigureServices(services =>
//            {
//                // Register your service here
//                services.AddScoped<IExcelReaderService, ExcelReaderService>();
//            })
//            .Build();

//        host.Run();
//    }
//}