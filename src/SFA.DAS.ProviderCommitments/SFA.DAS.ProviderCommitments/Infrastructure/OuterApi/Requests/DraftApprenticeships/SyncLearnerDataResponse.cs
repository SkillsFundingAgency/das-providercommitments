using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;

public class SyncLearnerDataResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public GetDraftApprenticeshipResponse UpdatedDraftApprenticeship { get; set; }
}
