﻿using AspNetCore.IServiceCollection.AddIUrlHelper;
using FluentValidation;
using OpenTelemetry.Logs;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization.Services;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.HealthChecks;
using SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

namespace SFA.DAS.ProviderCommitments.Web;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration.BuildDasConfiguration();
        _environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton(_configuration);

        services.AddLogging(builder =>
        {
            builder.AddFilter<OpenTelemetryLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<OpenTelemetryLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateCohortHandler>());

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = _ => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddConfigurationOptions(_configuration);

        services.AddDasHealthChecks();
        services.AddProviderAuthentication(_configuration);
        services.AddMemoryCache();
        services.AddCache(_environment, _configuration);
        services.AddModelMappings();

        services.AddDasMvc(_configuration);
        services.AddProviderUiServiceRegistration(_configuration);
        services.AddProviderFeatures();

        services.AddTransient<IAuthorizationService, AuthorizationService>();

        services
            .AddAuthorizationServices()
            .AddDataProtection(_configuration, _environment)
            .AddUrlHelper()
            .AddHealthChecks();

        BearerTokenProvider.SetSigningKey(_configuration["UserBearerTokenSigningKey"]);

        services
            .AddCommitmentsApiClient(_configuration)
            .AddApprovalsOuterApiClient()
            .AddProviderApprenticeshipsApiClient(_configuration);

        services.AddTransient<IValidator<CreateCohortRequest>, CreateCohortValidator>();
        services.AddTransient<IAuthenticationServiceForApim, AuthenticationService>();

        services.AddCommitmentPermissionsAuthorization(_configuration.UseLocalRegistry());

        services.AddEncodingServices(_configuration);
        services.AddApplicationServices();

        services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddHttpClient();
        services.AddOpenTelemetryRegistration(_configuration);
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        if (_environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.ConfigureCustomExceptionMiddleware()
            .UseStatusCodePagesWithReExecute("/error", "?statuscode={0}")
            .UseHttpsRedirection()
            .UseStaticFiles()
            .UseDasHealthChecks()
            .UseCookiePolicy()
            .UseAuthentication()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute())
            .UseHealthChecks("/health-check");
    }
}