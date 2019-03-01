using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using StructureMap.AspNetCore;

namespace SFA.DAS.ProviderCommitments.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureDasAppConfiguration()
                .ConfigureKestrel(options => options.AddServerHeader = false)
                .UseStructureMap()
                .UseStartup<Startup>()
                .UseNLog();
    }
}
