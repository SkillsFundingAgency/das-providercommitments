//using Microsoft.ApplicationInsights.Extensibility;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using SFA.DAS.CommitmentsV2.Api.Types.Validation;
//using SFA.DAS.CommitmentsV2.Shared.Extensions;
//using SFA.DAS.Validation.Mvc.Extensions;
//using SFA.DAS.Validation.Mvc.Filters;
//using SFA.DAS.Validation.Mvc.ModelBinding;
//using System.Threading.Channels;
//using System;
//using Microsoft.ApplicationInsights.Channel;

//namespace SFA.DAS.ProviderCommitments.Web;

//public static class ProviderCommitmentsMvcOptionsExtensions
//{
//    public static void AddValidationWithLogging(this MvcOptions mvcOptions)
//    {
//        mvcOptions.Filters.Add<DomainExceptionRedirectGetFilterWithLoggingAttribute>();
//        mvcOptions.Filters.Add<ValidateModelStateFilter>(int.MaxValue);
//    }
//}

//public class DomainExceptionRedirectGetFilterWithLoggingAttribute : ExceptionFilterAttribute
//{
//    private readonly ILogger<DomainExceptionRedirectGetFilterWithLoggingAttribute> _logger;

//    public DomainExceptionRedirectGetFilterWithLoggingAttribute(
//        ILogger<DomainExceptionRedirectGetFilterWithLoggingAttribute> logger)
//    {
//        _logger = logger;
//    }

//    public override void OnException(ExceptionContext context)
//    {
//        _logger.Log(LogLevel.Information, context.Exception, "FLP-202 OnException Triggered.");
//        if (!(context.Exception is CommitmentsApiModelException exception))
//            return;
//        ITempDataDictionary tempData = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(context.HttpContext);
//        ModelStateDictionary modelState = context.ModelState;
//        modelState.AddModelExceptionErrors(exception);
//        _logger.Log(LogLevel.Information, $"FLP-202 Exception errors added to model state: {context.ModelState}");
//        SerializableModelStateDictionary serializable = modelState.ToSerializable();
//        tempData.Set<SerializableModelStateDictionary>(serializable);
//        context.RouteData.Values.Merge(context.HttpContext.Request.Query);
//        _logger.Log(LogLevel.Information, $"FLP-202 Route Data Constructed: {context.RouteData}");
//        context.Result = (IActionResult)new RedirectToRouteResult((object)context.RouteData.Values);
//        _logger.Log(LogLevel.Information, $"FLP-202 Result Constructed: {context.Result}");
//    }

//    private void GetLogger()
//    {
//        using var channel = new InMemoryChannel();

//        IServiceCollection services = new ServiceCollection();
//        services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
//        services.AddLogging(builder =>
//        {
//            // Only Application Insights is registered as a logger provider
//            builder.AddApplicationInsights(
//                configureTelemetryConfiguration: (config) => config.ConnectionString = "<YourConnectionString>",
//                configureApplicationInsightsLoggerOptions: (options) => { }
//            );
//        });

//        IServiceProvider serviceProvider = services.BuildServiceProvider();
//        ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

//        logger.LogInformation("Logger is working...");
//    }
//}
