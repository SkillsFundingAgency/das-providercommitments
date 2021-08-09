using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DetailsViewCourseGroupingModel
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string DisplayCourseName => string.IsNullOrWhiteSpace(CourseName) ? "No training course" : CourseName;
        public int Count => DraftApprenticeships?.Count ?? 0;
        public FundingBandExcessModel FundingBandExcess { get; set; }
        public EmailOverlapsModel EmailOverlaps { get; set; }
        public IReadOnlyCollection<CohortDraftApprenticeshipViewModel> DraftApprenticeships { get; set; }
    }

    public class EmailOverlapsModel
    {
        public int NumberOfEmailOverlaps { get; }

        public EmailOverlapsModel(int numberOfEmailOverlaps)
        {
            NumberOfEmailOverlaps = numberOfEmailOverlaps;
        }

        public string DisplayEmailOverlapsMessage => NumberOfEmailOverlaps == 1
            ? "1 apprenticeship with an email issue"
            : $"{NumberOfEmailOverlaps} apprenticeships with email issues";
    }
}
