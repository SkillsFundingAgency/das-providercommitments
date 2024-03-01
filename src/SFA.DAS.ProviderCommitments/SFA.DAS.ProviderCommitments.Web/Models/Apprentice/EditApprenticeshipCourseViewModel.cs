using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class EditApprenticeshipCourseViewModel : IStandardSelection
    {
        public long ProviderId { get; set; }
        public string EmployerName { get; set; }
        public bool ShowManagingStandardsContent { get; set; }
        public string CourseCode { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public IEnumerable<Standard> Standards { get; set; }
    }
}
