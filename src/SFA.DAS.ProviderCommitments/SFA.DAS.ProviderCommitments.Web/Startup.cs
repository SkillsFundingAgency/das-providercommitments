using AspNetCore.IServiceCollection.AddIUrlHelper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.DependencyResolution;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.HealthChecks;
using SFA.DAS.ProviderCommitments.Web.Validators;
using StructureMap;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Mvc.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Infrastructure.CacheStorageService;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Services;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.ProviderCommitments.Web.Exceptions;

namespace SFA.DAS.ProviderCommitments.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.    
        public void ConfigureServices(IServiceCollection services)
        {
            var useDfeSignIn = Configuration.GetSection(ProviderCommitmentsConfigurationKeys.UseDfeSignIn).Get<bool>();
            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .AddHttpContextAccessor()
                .AddDasHealthChecks()
                .AddProviderAuthentication(Configuration)
                .AddMemoryCache()
                .AddCache(Environment, Configuration)
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.Filters.Add(new GoogleAnalyticsFilter());
                    options.AddProviderCommitmentsValidation();
                    options.Filters.Add(new AuthorizeFilter(PolicyNames.ProviderPolicyName));
                    options.Filters.Add<AuthorizationFilter>(int.MaxValue);
                    options.ModelBinderProviders.Insert(0, new SuppressArgumentExceptionModelBinderProvider());
                    options.ModelBinderProviders.Insert(1, new AuthorizationModelBinderProvider());
                    options.AddStringModelBinderProvider();
                })
                .AddNavigationBarSettings(Configuration)
                .EnableGoogleAnalytics()
                .EnableCookieBanner()
                .SetDfESignInConfiguration(useDfeSignIn)
                .AddZenDeskSettings(Configuration)
                .AddControllersAsServices()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddDraftApprenticeshipViewModelValidator>());
            
            services.AddScoped<HandleBulkUploadValidationErrorsAttribute>();
            services.AddScoped<DomainExceptionRedirectGetFilterAttribute>();
            services.AddScoped<ValidateModelStateFilter>();
            
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateCohortHandler>());

            services
                .AddAuthorizationService()
                .AddDataProtection(Configuration, Environment)
                .AddUrlHelper()
                .AddHealthChecks();

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            services.AddHttpClient();
            services.AddProviderUiServiceRegistration(Configuration);
            services.AddSingleton<IBlobFileTransferClient, BlobFileTransferClient>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddTransient<ICacheStorageService, CacheStorageService>();
            services.AddTransient<ITempDataStorageService, TempDataStorageService>();
            services.AddTransient<IOuterApiClient, OuterApiClient>();
            services.AddTransient<IOuterApiService, OuterApiService>();
        }

        public void ConfigureContainer(Registry registry)
        {
            IoC.Initialize(registry, Configuration);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error", "?statuscode={0}")
                .UseUnauthorizedAccessExceptionHandler()
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseDasHealthChecks()
                .UseCookiePolicy()
                .UseAuthentication()
                .UseAuthorization()
                .ConfigureCustomExceptionMiddleware()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                })
                .UseHealthChecks("/health-check");

            var logger = loggerFactory.CreateLogger(nameof(Startup));
            logger.Log(LogLevel.Information, "Application start up configure is complete");
        }
    }
}
