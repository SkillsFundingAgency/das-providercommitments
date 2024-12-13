namespace SFA.DAS.ProviderCommitments.Web.Exceptions;

public class UnauthorizedAccessExceptionMiddleware(RequestDelegate next, ILogger<UnauthorizedAccessExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex.Message);
            httpContext.Response.Redirect($"/error/404");
        }
    }
}
