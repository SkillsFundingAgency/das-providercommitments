using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Authorization.Commitments;
using SFA.DAS.ProviderCommitments.Web.Authorization.Handlers;
using SFA.DAS.ProviderCommitments.Web.Authorization.Provider;
using SFA.DAS.ProviderCommitments.Web.Authorization.Services;
using SFA.DAS.ProviderCommitments.Web.Caching;
using AuthorizationResultCache = SFA.DAS.ProviderCommitments.Web.Caching.AuthorizationResultCache;
using AuthorizationResultLogger = SFA.DAS.ProviderCommitments.Web.Authorization.AuthorizationResultLogger;
using IAuthorizationHandler = Microsoft.AspNetCore.Authorization.IAuthorizationHandler;
using IAuthorizationResultCacheConfigurationProvider = SFA.DAS.ProviderCommitments.Web.Caching.IAuthorizationResultCacheConfigurationProvider;
using ProviderAuthorizationHandler = SFA.DAS.ProviderCommitments.Web.Authorization.Provider.ProviderAuthorizationHandler;

namespace SFA.DAS.ProviderCommitments.Web.ServiceRegistrations;

public static class AuthorizationPolicy
{
    public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
    {
        AddAuthorizationPolicies(services);

        services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

        services.AddSingleton<ICommitmentsAuthorisationHandler, CommitmentsAuthorisationHandler>();
        services.AddSingleton<IProviderAuthorizationHandler, ProviderAuthorizationHandler>();

        services.AddTransient<IAuthorizationHandler, ProviderHandler>();
        services.AddTransient<IAuthorizationHandler, MinimumServiceClaimRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, AccessApprenticeshipAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, AccessCohortAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, CreateCohortAuthorizationHandler>();

        services.AddTransient<IAuthorizationContext, AuthorizationContext>();
        services.AddSingleton<IAuthorizationContextProvider, AuthorizationContextProvider>();

        services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();
        
        services.AddLogging()
            .AddMemoryCache()
            .AddScoped<IAuthorizationContextProvider>(p => new AuthorizationContextCache(p.GetService<DefaultAuthorizationContextProvider>()))
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddScoped<IDefaultAuthorizationHandler, DefaultAuthorizationHandler>()
            .AddScoped<DefaultAuthorizationContextProvider>()
            .AddScoped(p => p.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());

        services.Decorate<IAuthorizationService, AuthorizationServiceWithDefaultHandler>();
        
        return services;
    }

    private static void AddAuthorizationPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.ProviderPolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new ProviderRequirement());
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
            });

            options.AddPolicy(PolicyNames.HasViewerOrAbovePermission, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new MinimumServiceClaimRequirement(ServiceClaim.DAV));
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
            });

            options.AddPolicy(PolicyNames.HasContributorOrAbovePermission, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new MinimumServiceClaimRequirement(ServiceClaim.DAC));
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
            });

            options.AddPolicy(PolicyNames.HasContributorWithApprovalOrAbovePermission, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new MinimumServiceClaimRequirement(ServiceClaim.DAB));
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
            });

            options.AddPolicy(PolicyNames.HasAccountOwnerPermission, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new MinimumServiceClaimRequirement(ServiceClaim.DAA));
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
            });

            options.AddPolicy(PolicyNames.AccessApprenticeship, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new AccessApprenticeshipRequirement());
            });

            options.AddPolicy(PolicyNames.AccessCohort, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new AccessCohortRequirement());
            });

            options.AddPolicy(PolicyNames.CreateCohort, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.Service);
                policy.RequireClaim(ProviderClaims.Ukprn);
                policy.Requirements.Add(new CreateCohortRequirement());
            });
        });
    }

    public static IServiceCollection AddAuthorizationHandler<T>(this IServiceCollection services, bool enableAuthorizationResultCache = false) where T : class, SFA.DAS.ProviderCommitments.Web.Authorization.Handlers.IAuthorizationHandler
    {
        return services.AddScoped<T>().AddScoped(provider =>
        {
            var authorizationHandler = (Authorization.Handlers.IAuthorizationHandler)provider.GetService(typeof(T));
            var authorizationResultCacheConfigurationProviders = provider.GetServices<IAuthorizationResultCacheConfigurationProvider>();
            var memoryCache = provider.GetService<IMemoryCache>();
            var logger = provider.GetService<ILogger<AuthorizationResultLogger>>();

            if (enableAuthorizationResultCache)
            {
                authorizationHandler = new AuthorizationResultCache(authorizationHandler, authorizationResultCacheConfigurationProviders, memoryCache);
            }

            authorizationHandler = new AuthorizationResultLogger(authorizationHandler, logger);

            return authorizationHandler;
        });
    }
}