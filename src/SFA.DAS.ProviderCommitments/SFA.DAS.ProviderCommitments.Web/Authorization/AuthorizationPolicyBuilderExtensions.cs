using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.Authorization
{
    public static class AuthorizationPolicy
    {
        public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
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
            });

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IAuthorizationHandler, ProviderHandler>();
            services.AddTransient<IAuthorizationHandler, MinimumServiceClaimRequirementHandler>();
            services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();

            return services;
        }
    }
}