using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddProviderIdamsAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44381/";
                    options.ClientId = "openIdConnectClient";
                    //RedirectUri = "https://127.0.0.1:44347/signin-oidc",
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("idams");
                    options.ResponseType = "id_token";
                    options.UseTokenLifetime = false;
                    options.RequireHttpsMetadata = false;
                });

            return services;

            //var authenticationSettings = config.GetSection(ProviderCommitmentsConfigurationKeys.AuthenticationSettings).Get<AuthenticationSettings>();

            //services.AddAuthentication(sharedOptions =>
            //    {
            //        sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            //    })
            //    .AddWsFederation(options =>
            //    {
            //        // See: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/ws-federation?view=aspnetcore-2.2
            //        // This is the AAD tenant's "Federation Metadata Document" found on the app registrations blade
            //        options.MetadataAddress = authenticationSettings.MetadataAddress;
            //        // This is the app's "App ID URI" found in the app registration's Settings > Properties blade.
            //        options.Wtrealm = authenticationSettings.Wtrealm;
            //        options.Events.OnSecurityTokenValidated = OnSecurityTokenValidated;
            //    }).AddCookie(options => { options.ReturnUrlParameter = "/Home/Index"; });
            //return services;
        }

        private static Task OnSecurityTokenValidated(SecurityTokenValidatedContext context)
        {
            var claims = context.Principal.Claims;

            //todo: need to capture these values in the database via the api
            var ukprn = claims.FirstOrDefault(claim => claim.Type == (ProviderClaims.Ukprn))?.Value;
           

            return Task.CompletedTask;
        }
    }
}
