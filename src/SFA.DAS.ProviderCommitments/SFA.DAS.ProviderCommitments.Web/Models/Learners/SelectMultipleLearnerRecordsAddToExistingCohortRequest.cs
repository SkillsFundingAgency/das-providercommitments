using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Learners;

public class SelectMultipleLearnerRecordsAddToExistingCohortRequest : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string CohortReference { get; set; }
    public string AccountLegalEntityHashedId { get; set; }
    public string EncodedPledgeApplicationId { get; set; }
    public string TransferSenderHashedId { get; set; }
    public bool UseLearnerData { get; set; }
}