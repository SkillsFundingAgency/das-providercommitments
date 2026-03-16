namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class RecognisePriorLearningRequest : DraftApprenticeshipRequest
    {
    }

    public class RecognisePriorLearningViewModel : DraftApprenticeshipRequest
    {
        public bool? IsTherePriorLearning { get; set; }
        public bool RplNeedsToBeConsidered { get; set; }
        public bool IsRplRequired { get; set; }
    }
    
    public class PriorLearningDataViewModel : DraftApprenticeshipRequest
    {
        public int? PriceReduced { get; set; }
        public int? TrainingTotalHours { get; set; }
        public int? DurationReducedByHours { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? DurationReducedBy { get; set; } // by Weeks
        public int? CostBeforeRpl { get; set; }
        public long? LearnerDataId { get; set; }
    }

    public class RecognisePriorLearningResult : DraftApprenticeshipRequest
    {
        public bool RplPriceReductionError { get; set; }
    }
    public class PriorLearningSummaryRequest : DraftApprenticeshipRequest
    {
    }

    public class PriorLearningSummaryViewModel : DraftApprenticeshipRequest
    {
        public int? TrainingTotalHours { get; set; }
        public int? DurationReducedByHours { get; set; }
        public int? CostBeforeRpl { get; set; }
        public int? PriceReducedBy { get; set; }
        public int? FundingBandMaximum { get; set; }
        public decimal? PercentageOfPriorLearning { get; set; }
        public decimal? MinimumPercentageReduction { get; set; }
        public int? MinimumPriceReduction { get; set; }
        public bool? RplPriceReductionError { get; set; }
        public int? TotalCost { get; set; }
        public string FullName { get; set; }
        public bool HasStandardOptions { get; set; }
        public double? PercentageTotalTraining => (double)DurationReducedByHours.GetValueOrDefault() / (double)TrainingTotalHours.GetValueOrDefault() * 100;
        public double? PercentageMinimumFunding => PercentageTotalTraining / 2;
        public long? LearnerDataId { get; set; }
    }
}