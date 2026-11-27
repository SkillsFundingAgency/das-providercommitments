using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

public class SelectMultipleAddDraftApprenticeshipsApimRequest : ApimSaveDataRequest
{
    public long ProviderId { get; set; }
    public IEnumerable<long> LearnerIds { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string AgreementId { get; set; }
    public long AccountId { get; set; }
    public long? CohortId { get; set; }
    public long? TransferSenderAccountId { get; set; }
    public string CohortReference { get; set; }
    public string EncodedPledgeApplicationId { get; set; }
}