using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class ChangeHistoryRequest : IAuthorizationContextModel
{
    [FromRoute]
    public string ApprenticeshipHashedId { get; set; }

    [FromRoute]
    public long ProviderId { get; set; }
}
