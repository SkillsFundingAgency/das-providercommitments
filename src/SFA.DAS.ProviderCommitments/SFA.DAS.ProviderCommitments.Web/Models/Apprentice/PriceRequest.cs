namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceRequest
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string StartDate { get; set; }
        public string StopDate { get; set; }
    }
}
