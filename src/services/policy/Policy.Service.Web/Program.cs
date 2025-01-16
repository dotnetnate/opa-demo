using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;


namespace CitizensFinancialGroup.Threvw.Policies.Service.Http {

    [ExcludeFromCodeCoverage]
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {                    
                    webBuilder.UseStartup<Startup>();                                        
                });
    }
}