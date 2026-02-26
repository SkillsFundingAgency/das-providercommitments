using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ViewSelectOptionsViewModel : IAuthorizationContextModel
    {
        public List<string> Options { get ; set ; }
        public long CohortId { get ; set ; }
        public string CohortReference { get; set; }
        public long DraftApprenticeshipId { get ; set ; }
        public string DraftApprenticeshipHashedId { get ; set ; }
        public long ProviderId { get ; set ; }
        public string TrainingCourseName { get ; set ; }
        public string TrainingCourseVersion { get ; set ; }
        public string SelectedOption { get ; set ; }
        public string StandardPageUrl { get; set; }
        public bool? HasSelectedRpl { get; set; }
        public DateTime? ApprenticeshipStartDate { get; set; }
        public long? LearnerDataId { get; set; }
        public string OriginalSelectedOption { get; set; }

        public string DisplayUpdateMessage()
        {
            if (string.IsNullOrEmpty(OriginalSelectedOption) && !string.IsNullOrEmpty(SelectedOption))
                return "Reference added";
            if (!string.IsNullOrEmpty(OriginalSelectedOption) && string.IsNullOrEmpty(SelectedOption))
                return "Reference removed";
            if (OriginalSelectedOption != SelectedOption)
                return "Reference changed";
            return null;
        }
        public bool HasChanged() => SelectedOption != OriginalSelectedOption;
    }
}