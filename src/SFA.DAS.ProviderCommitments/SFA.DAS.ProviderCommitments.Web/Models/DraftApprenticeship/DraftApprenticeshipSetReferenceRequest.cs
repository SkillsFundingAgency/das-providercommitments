using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequest
{
    public string Reference { get; set; }
    public string Name { get; set; }
    public Party Party { get; set; }
    public long DraftApprenticeshipId { get; set; }
    public long CohortId {  get; set; }
    public long ProviderId { get; set; }
    public string DraftApprenticeshipHashedId {  get; set; }
}
