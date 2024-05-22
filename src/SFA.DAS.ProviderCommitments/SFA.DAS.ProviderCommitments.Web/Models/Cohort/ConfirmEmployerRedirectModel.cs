using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class ConfirmEmployerRedirectModel : IAuthorizationContextModel
    {
        public bool HasNoDeclaredStandards { get; set; }
    }
}
