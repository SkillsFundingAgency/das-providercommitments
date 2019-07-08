using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    //todo: this could be a shared package
    public static class ProviderIdamsStubIServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services necessary for authentication using provider idams stub
        /// </summary>
        /// <param name="services"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IServiceCollection UseProviderIdamsStubAuthentication(this IServiceCollection services, OpenIdConnectEvents events=null)
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
                    options.Scope.Add("openid");
                    options.Scope.Add("idams");
                    options.ResponseType = "id_token";
                    options.UseTokenLifetime = false;
                    options.RequireHttpsMetadata = false;
                    options.Events = events;
                });

            return services;
        }

    }
}
