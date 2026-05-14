using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class BeforeYouContinueMultiSelectRequest : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
}