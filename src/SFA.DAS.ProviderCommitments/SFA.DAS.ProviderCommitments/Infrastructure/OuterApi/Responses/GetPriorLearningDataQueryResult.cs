namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetPriorLearningDataQueryResult
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
        public int? PriceReduced { get; set; }
        public int? TrainingTotalHours { get; set; }
        public int? DurationReducedByHours { get; set; }
        public int? ReducedDuration { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? DurationReducedBy { get; set; } // by Weeks
        public int? CostBeforeRpl { get; set; }
    }
}
