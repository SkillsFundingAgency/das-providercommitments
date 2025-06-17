namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetRplRequirementsRequest(
        long providerId,
        long cohortId,
        long draftApprenticeshipId,
        string courseCode)
        : IGetApiRequest
    {
        public string GetUrl => $"provider/{providerId}/unapproved/{cohortId}/apprentices/{draftApprenticeshipId}/recognise-prior-learning?courseId={courseCode}";
    }
} 