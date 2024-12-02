namespace SFA.DAS.ProviderCommitments.Web.Exceptions
{
    public static class ExceptionMiddleWareExtensions
    {
        public static IApplicationBuilder ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CreateCohortCacheItemNotFoundExceptionMiddleware>();
            app.UseMiddleware<ChangeEmployerCacheItemNotFoundExceptionMiddleware>();
            app.UseMiddleware<UnauthorizedAccessExceptionMiddleware>();
            app.UseMiddleware<UnauthorizedOptionMiddleware>();
            return app;
        }
    }
}
