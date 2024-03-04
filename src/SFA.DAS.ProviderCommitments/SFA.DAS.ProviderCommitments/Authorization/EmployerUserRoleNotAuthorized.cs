namespace SFA.DAS.ProviderCommitments.Authorization
{
    public class EmployerUserRoleNotAuthorized : AuthorizationError
    {
        public EmployerUserRoleNotAuthorized() : base("Employer user role is not authorized")
        {
        }
    }
}