using FluentValidation.AspNetCore;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Mvc.ModelBinding;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Filters;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.ProviderCommitments.Web.Validators;
using SFA.DAS.Validation.Mvc.Filters;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class MvcServiceRegistrations
{
    public static IServiceCollection AddDasMvc(this IServiceCollection services, IConfiguration configuration)
    {
        var useDfeSignIn = configuration.GetSection(ProviderCommitmentsConfigurationKeys.UseDfeSignIn).Get<bool>();
        
        services.AddMvc(options =>
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
            .SetDefaultNavigationSection(NavigationSection.YourCohorts)
            .EnableGoogleAnalytics()
            .EnableCookieBanner()
            .SetDfESignInConfiguration(useDfeSignIn)
            .AddZenDeskSettings(configuration)
            .AddControllersAsServices()
            .AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<AddDraftApprenticeshipViewModelValidator>());

        services
            .AddScoped<HandleBulkUploadValidationErrorsAttribute>()
            .AddScoped<DomainExceptionRedirectGetFilterAttribute>()
            .AddScoped<ValidateModelStateFilter>();

        return services;
    }
}