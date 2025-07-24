using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

public class SelectAddAnotherDraftApprenticeshipJourneyViewModel : IAuthorizationContextModel
{
    [FromRoute]
    public long ProviderId { get; set; }
    [FromRoute]
    public string CohortReference { get; set; }
    public long? CohortId { get; set; }
    public string AccountLegalEntityHashedId { get; set; }
    public string EncodedPledgeApplicationId { get; set; }
    public string TransferSenderHashedId { get; set; }
    public AddAnotherDraftApprenticeshipJourneyOptions? Selection { get; set; }
}

public enum AddAnotherDraftApprenticeshipJourneyOptions
{
    Ilr
}