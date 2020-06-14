using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Scraper.Configuration;

namespace Scraper.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(_ =>
            {
                _
                    .AddScrpr();
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>().UseUrls("http://*:5080"));
    }
}
