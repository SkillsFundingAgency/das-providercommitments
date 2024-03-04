namespace SFA.DAS.ProviderCommitments.Web.Exceptions
{
    public static class ExceptionMiddleWareExtensions
    {
        public static IApplicationBuilder ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CreateCohortCacheItemNotFoundExceptionMiddleware>();
            app.UseMiddleware<ChangeEmployerCacheItemNotFoundExceptionMiddleware>();
            return app;
        }
    }
}
