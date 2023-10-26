using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.ProviderCommitments.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Exceptions;

public class ChangeEmployerCacheItemNotFoundExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ChangeEmployerCacheItemNotFoundExceptionMiddleware(RequestDelegate next)
    {

        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CacheItemNotFoundException<ChangeEmployerCacheItem>)
        {
            var providerId = httpContext.GetRouteValue("ProviderId");
            var apprenticeshipId = httpContext.GetRouteValue("ApprenticeshipHashedId");
            httpContext.Response.Redirect($"/{providerId}/apprentices/{apprenticeshipId}/change-employer/inform");
        }
    }
}