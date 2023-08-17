using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Authentication
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => CustomClaimsIdentity.Service;
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Code;
    }
}
