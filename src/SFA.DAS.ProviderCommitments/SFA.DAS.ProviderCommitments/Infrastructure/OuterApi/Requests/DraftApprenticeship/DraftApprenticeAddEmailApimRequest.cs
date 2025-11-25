using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

public class DraftApprenticeAddEmailApimRequest : SaveDataRequest
{
    public string Email { get; set; }
}
