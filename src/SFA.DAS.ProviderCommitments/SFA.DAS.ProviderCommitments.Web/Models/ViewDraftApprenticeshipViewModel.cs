using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ViewDraftApprenticeshipViewModel : IDraftApprenticeshipViewModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Uln { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TrainingCourse { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmploymentPrice { get; set; }
        public int? Cost { get; set; }
        public string Reference { get; set; }
        public string TrainingCourseVersion { get ; set ; }
        public string TrainingCourseOption { get ; set ; }
        public bool HasTrainingCourseOption { get; set; }
        public bool? EmailAddressConfirmed { get; set; }
        public bool? RecognisePriorLearning { get; set; }
        public bool RecognisingPriorLearningStillNeedsToBeConsidered { get; set; }
        public bool RecognisingPriorLearningExtendedStillNeedsToBeConsidered { get; set; }
        public int? DurationReducedBy { get; set; }
        public int? PriceReducedBy { get; set; }
        public int? DurationReducedByHours { get; set; }
        public bool? IsDurationReducedByRpl { get; set; }
        public int? TrainingTotalHours { get; set; }


    }
}