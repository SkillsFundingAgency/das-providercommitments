namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceViewModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string StartDate { get; set; }
        public decimal? Price { get; set; }
    }
}
