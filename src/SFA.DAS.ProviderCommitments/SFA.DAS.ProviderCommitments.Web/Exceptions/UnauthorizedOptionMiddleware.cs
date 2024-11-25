using SFA.DAS.ProviderCommitments.Exceptions;

namespace SFA.DAS.ProviderCommitments.Web.Exceptions;

public class UnauthorizedOptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedActionException)
        {
            var redirectUrl = $"/error/403?isActionRequest=true";
            context.Response.Redirect(redirectUrl);
        }
    }
}
