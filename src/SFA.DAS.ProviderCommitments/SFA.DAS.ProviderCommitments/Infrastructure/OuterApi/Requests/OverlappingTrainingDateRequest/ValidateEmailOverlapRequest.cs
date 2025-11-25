using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;

public class ValidateEmailOverlapRequest : IGetApiRequest
{
    public  long DraftApprenticeshipId { get; set; }
    public string Email { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }

    public long CohortId { get; set; }

    public ValidateEmailOverlapRequest(long draftApprenticeshipId, string email, string startDate, string endDate, long cohortId)
    {
        DraftApprenticeshipId = draftApprenticeshipId;
        Email = email;
        StartDate = startDate;
        EndDate = endDate;
        CohortId = cohortId;
    }
    public string GetUrl => $"OverlappingTrainingDateRequest/{DraftApprenticeshipId}/validateEmailOverlap?email={Email}&startDate={StartDate}&endDate={EndDate}&CohortId={CohortId}";
}
