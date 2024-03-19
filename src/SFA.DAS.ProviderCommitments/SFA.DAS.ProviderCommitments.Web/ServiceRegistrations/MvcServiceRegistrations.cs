using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new GoogleAnalyticsFilter());
                options.AddProviderCommitmentsValidation();
                options.Filters.Add(new AuthorizeFilter(PolicyNames.ProviderPolicyName));
                options.ModelBinderProviders.Insert(0, new SuppressArgumentExceptionModelBinderProvider());
                options.ModelBinderProviders.Insert(1, new AuthorizationModelBinderProvider());
            })
            .SetDefaultNavigationSection(NavigationSection.YourCohorts)
            .EnableGoogleAnalytics()
            .EnableCookieBanner()
            .SetDfESignInConfiguration(useDfeSignIn)
            .AddZenDeskSettings(configuration)
            .AddControllersAsServices();

        services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<AddDraftApprenticeshipViewModelValidator>();

        services
            .AddScoped<HandleBulkUploadValidationErrorsAttribute>()
            .AddScoped<UseCacheForValidationAttribute>()
            .AddScoped<DomainExceptionRedirectGetFilterAttribute>()
            .AddScoped<ValidateModelStateFilter>();

        services.AddScoped<IUrlHelper>(sp =>
        {
            var actionContext = sp.GetService<IActionContextAccessor>().ActionContext;
            return new UrlHelper(actionContext);
        });

        return services;
    }
}