using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class AddDraftApprenticeshipCourseViewModel : IStandardSelection
    {
        public long ProviderId { get; set; }
        public string EmployerName { get; set; }
        public bool ShowManagingStandardsContent { get; set; }
        public string CourseCode { get; set; }
        public IEnumerable<Standard> Standards { get; set; }
    }
}
