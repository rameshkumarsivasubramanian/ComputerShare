using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Web.UI.Services;

namespace Web.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create document repository folder if it doesn't exist
            FileManager.CreateDocumentsFolder();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
