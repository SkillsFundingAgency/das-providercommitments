using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

public class PostDraftApprenticeshipSetReferenceApimRequest
{
    public string Reference {  get; set; }

    public Party Party { get; set; }
}
