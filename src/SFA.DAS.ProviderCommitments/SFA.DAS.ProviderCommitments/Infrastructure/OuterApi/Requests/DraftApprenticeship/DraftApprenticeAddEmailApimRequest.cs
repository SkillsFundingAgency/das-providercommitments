namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;

public class DraftApprenticeAddEmailApimRequest 
{
    public string Email { get; set; }
    public long CohortId { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}
