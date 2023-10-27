using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.ProviderCommitments.Configuration;

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
                var useDfeSignIn = config.GetSection(ProviderCommitmentsConfigurationKeys.UseDfeSignIn).Get<bool>();
                if (useDfeSignIn)
                {
                    services.AddAndConfigureDfESignInAuthentication(
                        config,
                        CookieAuthName,
                        typeof(CustomServiceRole),
                        ClientName.ProviderRoatp,
                        "/signout",
                        "");
                }
                else
                {
                    services.AddProviderIdamsAuthentication(config);
                }
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
            });
            return services;
        }

        private static IServiceCollection AddProviderIdamsAuthentication(this IServiceCollection services, IConfiguration config)
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
