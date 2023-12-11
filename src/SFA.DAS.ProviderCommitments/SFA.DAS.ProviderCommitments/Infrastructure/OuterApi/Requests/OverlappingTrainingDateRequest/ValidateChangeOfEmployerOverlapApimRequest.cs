namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest
{
    public class ValidateChangeOfEmployerOverlapApimRequest
    {
        public string Uln { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long ProviderId { get; set; }
    }
}
