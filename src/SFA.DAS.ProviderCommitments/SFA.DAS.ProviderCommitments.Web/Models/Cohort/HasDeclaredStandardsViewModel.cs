using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class HasDeclaredStandardsViewModel : IAuthorizationContextModel
    {
        public bool HasNoDeclaredStandards { get; set; }
    }
}
