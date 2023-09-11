namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetPriorLearningDataQueryResult
    {
        public int? PriceReduced { get; set; }
        public int? TrainingTotalHours { get; set; }
        public int? DurationReducedByHours { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? DurationReducedBy { get; set; } // by Weeks
        public int? CostBeforeRpl { get; set; }
    }
}