using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.AppStart
{
    public static class AuthenticationExtensions
    {
        public static void AddProviderIdamsAuthentication(this IServiceCollection services, IOptions<AuthenticationSettings> configuration)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = configuration.Value.MetadataAddress;
                    options.Wtrealm = configuration.Value.Wtrealm;
                    options.Events.OnSecurityTokenValidated = context => Task.CompletedTask;
                }).AddCookie(options => { options.ReturnUrlParameter = "/Home/Index"; });
        }
    }
}
