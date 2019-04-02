using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseFeatureToggles(this IApplicationBuilder app)
        {
            var routeBuilder = new RouteBuilder(app).RemoveDisabledFeatures(app.ApplicationServices.GetService<IFeatures>());
            app.UseRouter(routeBuilder.Build());
            return app;
        }
    }
}