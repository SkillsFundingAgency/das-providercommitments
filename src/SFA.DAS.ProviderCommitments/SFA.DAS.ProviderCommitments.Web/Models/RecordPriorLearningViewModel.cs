using System;

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
        public double? DurationReducedByHours { get; set; }
        public double? WeightageReducedBy { get; set; }
        public string Qualification { get; set; }
        public string Reason { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    public class RecognisePriorLearningResult : DraftApprenticeshipRequest
    {
        public bool HasStandardOptions { get; set; }
    }
}