namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ChangePriceViewModel
    {
        public long ProviderId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string NewStartDate { get; set; }
        public decimal? NewPrice { get; set; }
    }
}
