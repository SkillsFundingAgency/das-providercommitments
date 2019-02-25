using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog.Web;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Authentication;
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

            services.AddDasConfigurationSections(Configuration);

            var authenticationSettingsSection = Configuration.GetSection("SFA.DAS.ProviderCommitments:AuthenticationSettings");

            var authenticationSettings = authenticationSettingsSection.Get<AuthenticationSettings>();

            services.AddProviderIdamsAuthentication(authenticationSettings);

            services.AddMvc(options => { options.Filters.Add(new AuthorizeFilter()); })
                .AddControllersAsServices()
                .AddSessionStateTempDataProvider()
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //todo: app insights key

            var container = CreateStructureMapContainer(services);

            return container.GetInstance<IServiceProvider>();
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

        public void ConfigureContainer(Registry registry)
        {
            registry.IncludeRegistry<DefaultRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
        }

        private static Container CreateStructureMapContainer(IServiceCollection services)
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.AddRegistry<DefaultRegistry>();
                config.AddRegistry<ConfigurationRegistry>();
                config.Populate(services);
            });

            return container;
        }
    }
}
