using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder RemoveDisabledFeatures(this IRouteBuilder routeBuilder, IFeatures features)
        {
            foreach (var disabledEndPoint in features.DisabledFeatures.SelectMany(feature => feature.EndPoints))
            {
                routeBuilder.MapRoute(disabledEndPoint, ctx =>
                {
                    ctx.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return ctx.Response.WriteAsync("Feature not enabled");
                });
            }

            return routeBuilder;
        }
    }
}