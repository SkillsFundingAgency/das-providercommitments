namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class RecognisePriorLearningRequest : DraftApprenticeshipRequest
    {
    }

    public class RecognisePriorLearningViewModel : DraftApprenticeshipRequest
    {
        public bool? IsTherePriorLearning { get; set; }
    }

    public class PriorLearningDetailsViewModel : DraftApprenticeshipRequest
    {
        public int? ReducedPrice { get; set; }
        public int? ReducedDuration { get; set; } 
        public int? DurationReducedByHours { get; set; }
        public int? WeightageReducedBy { get; set; }
        public string QualificationsForRplReduction { get; set; }
        public string ReasonForRplReduction { get; set; }
       
    }

    public class PriorLearningDataViewModel : DraftApprenticeshipRequest
    {
        public int? PriceReduced { get; set; }
        public int? TrainingTotalHours { get; set; }
        public int? DurationReducedByHours { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? DurationReducedBy { get; set; } // by Weeks
        public int? CostBeforeRpl { get; set; }
    }

    public class RecognisePriorLearningResult : DraftApprenticeshipRequest
    {
        public bool HasStandardOptions { get; set; }
    }
    public class PriorLearningSummaryRequest : DraftApprenticeshipRequest
    {
    }

    public class PriorLearningSummaryViewModel : DraftApprenticeshipRequest
    {
        public bool HasStandardOptions { get; set; }
        public bool? IsAllowed { get; set; }
    }
    public class PriorLearningSummaryResult : DraftApprenticeshipRequest
    {
        public bool IsAllowed { get; set; }
        public bool HasStandardOptions { get; set; }

    }
}