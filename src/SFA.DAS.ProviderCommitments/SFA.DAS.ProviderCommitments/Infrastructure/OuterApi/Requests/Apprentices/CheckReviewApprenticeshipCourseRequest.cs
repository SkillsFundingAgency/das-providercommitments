namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class CheckReviewApprenticeshipCourseRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public CheckReviewApprenticeshipCourseRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/changes/review";
    }

    public class CheckReviewApprenticeshipCourseResponse
    {
        public bool IsValidCourseCode { get; set; }
    }
}
