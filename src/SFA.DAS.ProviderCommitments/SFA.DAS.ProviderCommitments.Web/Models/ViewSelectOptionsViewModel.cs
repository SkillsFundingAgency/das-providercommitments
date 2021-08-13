using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ViewSelectOptionsViewModel
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
        public string StandardPageUrl { get ; set ; }
    }
}