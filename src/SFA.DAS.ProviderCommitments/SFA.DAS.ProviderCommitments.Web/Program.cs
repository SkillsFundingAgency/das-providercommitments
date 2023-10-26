using System;
using NLog.Web;

namespace SFA.DAS.ProviderCommitments.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var logger = NLogBuilder.ConfigureNLog(environment == "Development" ? "nlog.Development.config" :"nlog.config").GetCurrentClassLogger();
        logger.Info("Starting up host");

        CreateWebHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseNLog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}