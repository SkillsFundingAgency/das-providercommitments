namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class PostCohortDetailsRequest : IPostApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; }

        public PostCohortDetailsRequest(long providerId, long cohortId, Body body)
        {
            Data = body;
            ProviderId = providerId;
            CohortId = cohortId;
        }

        public string PostUrl => $"provider/{ProviderId}/unapproved/{CohortId}";
        public object Data { get; set; }

        public class Body : ApimSaveDataRequest
        {
            public CohortSubmissionType SubmissionType { get; set; }
            public string Message { get; set; }
        }

        public enum CohortSubmissionType
        {
            Send,
            Approve
        }
    }

    public class PostCohortDetailsResponse
    {
    }
}
