using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.ProviderCommitments.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Exceptions;

public class CreateCohortCacheItemNotFoundExceptionMiddleware
{
    private readonly RequestDelegate _next;
        
    public CreateCohortCacheItemNotFoundExceptionMiddleware(RequestDelegate next)
    {
        
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CacheItemNotFoundException<CreateCohortCacheItem>)
        {
            var providerId = httpContext.GetRouteValue("ProviderId");
            httpContext.Response.Redirect($"/{providerId}/unapproved/add/select-employer");
        }
    }
}