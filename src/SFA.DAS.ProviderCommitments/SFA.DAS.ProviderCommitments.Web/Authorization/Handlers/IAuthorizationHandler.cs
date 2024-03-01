namespace SFA.DAS.ProviderCommitments.Web.Authorization.Handlers
{
    public interface IAuthorizationHandler : IDefaultAuthorizationHandler
    {
        string Prefix { get; }
    }
}