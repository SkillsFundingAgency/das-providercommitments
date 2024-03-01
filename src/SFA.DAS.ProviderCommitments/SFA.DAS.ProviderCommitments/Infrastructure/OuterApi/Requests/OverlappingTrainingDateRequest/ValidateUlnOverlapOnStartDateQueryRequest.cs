namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class ValidateUlnOverlapOnStartDateQueryRequest : IGetApiRequest
    {
        public readonly long ProviderId;
        public string Uln { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public ValidateUlnOverlapOnStartDateQueryRequest(long providerId, string uln, string startDate, string endDate)
        {
            ProviderId = providerId;
            Uln = uln;
            StartDate = startDate;
            EndDate = endDate;
        }
        public string GetUrl => $"OverlappingTrainingDateRequest/{ProviderId}/validateUlnOverlap?uln={Uln}&startDate={StartDate}&endDate={EndDate}";
    }
}
