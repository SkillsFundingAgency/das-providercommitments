using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public static class AuthenticationExtensions
    {
        private const string CookieAuthName = "SFA.DAS.ProviderApprenticeshipService";

        public static IServiceCollection AddProviderAuthentication(this IServiceCollection services, IConfiguration config)
        {
            if (config["UseStubProviderAuth"] != null && bool.Parse(config["UseStubProviderAuth"]))
            {
                services.AddProviderStubAuthentication();
            }
            else
            {
                services.AddAndConfigureDfESignInAuthentication(
                        config,
                        CookieAuthName,
                        typeof(CustomServiceRole),
                        ClientName.ProviderRoatp,
                        "/signout",
                        "");
            }

            return services;
        }

        private static IServiceCollection AddProviderStubAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Provider-stub").AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(
                "Provider-stub",
                options => { }).AddCookie(options =>
            {
                options.AccessDeniedPath = "/Error/403";
                options.CookieManager = new ChunkingCookieManager {ChunkSize = 3000};
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ReturnUrlParameter = "/Home/Index";
                options.Cookie.HttpOnly = true;
            });
            return services;
        }

        public class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public ProviderStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
            {
                _httpContextAccessor = httpContextAccessor;
            }
 
            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "10005077"),
                    new Claim(ProviderClaims.DisplayName, "Test-U-Good Corporation"),
                    new Claim(ProviderClaims.Service, "DAA"),
                    new Claim(ProviderClaims.Ukprn, "10005077"),
                    new Claim(ProviderClaims.Upn, "10005077"),
                    new Claim(ProviderClaims.Email, "test+10005077@test.com"),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Provider")
                };
                var identity = new ClaimsIdentity(claims, "Provider-stub");
                
                var principal = new ClaimsPrincipal(identity);
                
                var ticket = new AuthenticationTicket(principal, "Provider-stub");
 
                var result = AuthenticateResult.Success(ticket);
 
                _httpContextAccessor.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType,"10005077");
                _httpContextAccessor.HttpContext.Items.Add(ClaimsIdentity.DefaultRoleClaimType,"Provider");
                _httpContextAccessor.HttpContext.Items.Add(ProviderClaims.DisplayName,"Test-U-Good Corporation");
            
                return Task.FromResult(result);
            }
        }
    }
    
    
}
