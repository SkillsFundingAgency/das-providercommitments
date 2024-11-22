namespace SFA.DAS.ProviderCommitments.Web.Exceptions;

public class UnauthorizedAccessExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (UnauthorizedAccessException)
        {
            httpContext.Response.Redirect($"/error/404");
        }
    }
}
