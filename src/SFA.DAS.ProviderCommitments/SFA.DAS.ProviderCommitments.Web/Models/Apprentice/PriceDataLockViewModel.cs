namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class PriceDataLockViewModel
    {
        public long ApprenticeshipId { get; set; }

        public decimal CurrentCost { get; set; }

        public DateTime CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }

        public decimal? IlrTotalCost { get; set; }
    }
}
