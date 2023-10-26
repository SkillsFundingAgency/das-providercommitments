using System;
using Microsoft.AspNetCore;
using NLog.Web;
using StructureMap.AspNetCore;

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
    
    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseNLog()
            .UseStructureMap()
            .UseStartup<Startup>();
}