using System;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class RecognisePriorLearningRequest : DraftApprenticeshipRequest
    {
    }

    public class RecognisePriorLearningViewModel : DraftApprenticeshipRequest
    {
        public int RplQualRating { get; set; }
        public bool? IsTherePriorLearning { get; set; }
        public RplOpenAiService.CourseResponse RplCourseResponse { get; set; }
        public int TypicalCourseHours { get; set; }
        public string CourseName { get; set; }
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

    public class RecognisePriorLearningResult : DraftApprenticeshipRequest
    {
        public bool HasStandardOptions { get; set; }
    }
}