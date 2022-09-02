namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship
{
    public class GetAddDraftApprenticeshipDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long CohortId { get; set; }
        public string CourseCode { get; set; }

        public GetAddDraftApprenticeshipDetailsRequest(long providerId, long cohortId, string courseCode)
        {
            ProviderId = providerId;
            CohortId = cohortId;
            CourseCode = courseCode;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/{CohortId}/apprentices/add/details?courseCode={CourseCode}";
    }

    public class GetAddDraftApprenticeshipDetailsResponse
    {
        public long AccountLegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }

    }
}
