using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class HttpContextExtensions
{
    public static bool TryGetValueFromHttpContext(this HttpContext httpContext, string key, out string value)
    {
        value = null;

        // for testing
        if (httpContext == null)
        {
            return false;
        }

        if (httpContext.GetRouteData().Values.TryGetValue(key, out var routeValue))
        {
            value = (string)routeValue;
        }
        else if (httpContext.Request.Query.TryGetValue(key, out var queryStringValue))
        {
            value = queryStringValue;
        }
        else if (httpContext.Request.HasFormContentType && httpContext.Request.Form.TryGetValue(key, out var formValue))
        {
            value = formValue;
        }

        return !string.IsNullOrWhiteSpace(value);
    }
}