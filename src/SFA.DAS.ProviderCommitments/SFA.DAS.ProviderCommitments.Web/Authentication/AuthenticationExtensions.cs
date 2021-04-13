using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddProviderIdamsAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var authenticationSettings = config.GetSection(ProviderCommitmentsConfigurationKeys.AuthenticationSettings).Get<AuthenticationSettings>();

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = authenticationSettings.MetadataAddress;
                    options.Wtrealm = authenticationSettings.Wtrealm;
                    options.Events.OnSecurityTokenValidated = OnSecurityTokenValidated;
                }).AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Error/403";
                    options.CookieManager = new ChunkingCookieManager {ChunkSize = 3000};
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.ReturnUrlParameter = "/Home/Index";
                });
            return services;
        }

        private static Task OnSecurityTokenValidated(SecurityTokenValidatedContext context)
        {
            var claims = context.Principal.Claims;
            var ukprn = claims.FirstOrDefault(claim => claim.Type == (ProviderClaims.Ukprn))?.Value;

            return Task.CompletedTask;
        }
    }
}
