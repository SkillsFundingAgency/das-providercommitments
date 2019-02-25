using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog.Web;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Authorisation;
using SFA.DAS.ProviderCommitments.Web.DependencyResolution;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions();
            services.Configure<AuthenticationSettings>(Configuration.GetSection("AuthenticationSettings"));

            var authenticationSettings = services.BuildServiceProvider().GetService<IOptions<AuthenticationSettings>>();

            services.AddProviderIdamsAuthentication(authenticationSettings);

            services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireProviderInRouteMatchesProviderInClaims()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddControllersAsServices()
                .AddSessionStateTempDataProvider()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                
            ConfigureAuthorization(services);

            //todo: app insights key

            var container = CreateStructureMapContainer(services);
            return container.GetInstance<IServiceProvider>();
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProviderMatch", policy => policy.Requirements.Add(new ProviderRequirement()));
            });

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IAuthorizationHandler, ProviderHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            env.ConfigureNLog("nlog.config");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static Container CreateStructureMapContainer(IServiceCollection services)
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.AddRegistry(new DefaultRegistry());
                config.Populate(services);
            });

            return container;
        }
    }
}
