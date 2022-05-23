namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class RecognisePriorLearningRequest : DraftApprenticeshipRequest
    {
    }

    public class RecognisePriorLearningViewModel : DraftApprenticeshipRequest
    {
        public bool? IsTherePriorLearning { get; set; }
        public int? ReducedPrice { get; set; }
        public int? ReducedDuration { get; set; }
    }

    public class RecognisePriorLearningResult : DraftApprenticeshipRequest
    {
        public bool HasStandardOptions { get; set; }
    }
}