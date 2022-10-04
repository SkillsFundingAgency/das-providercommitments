namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts
{
    public class GetAddDraftApprenticeshipDetailsRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long AccountLegalEntityId { get; set; }
        public string CourseCode { get; set; }

        public GetAddDraftApprenticeshipDetailsRequest(long providerId, long accountLegalEntityId, string courseCode)
        {
            ProviderId = providerId;
            AccountLegalEntityId = accountLegalEntityId;
            CourseCode = courseCode;
        }

        public string GetUrl => $"provider/{ProviderId}/unapproved/add/apprenticeship?accountLegalEntityId={AccountLegalEntityId}&courseCode={CourseCode}";
    }

    public class GetAddDraftApprenticeshipDetailsResponse
    {
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public bool HasMultipleDeliveryModelOptions { get; set; }

    }
}
