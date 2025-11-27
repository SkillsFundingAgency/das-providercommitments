using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequest: DraftApprenticeshipRequest
{
    public string Reference { get; set; }
    public string Name { get; set; }
    public Party Party { get; set; }
}

