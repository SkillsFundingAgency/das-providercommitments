using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

public class SelectCourseViewModel : IStandardSelection
{
    public long ProviderId { get; set; }
    public string EmployerName { get; set; }
    public bool ShowManagingStandardsContent { get; set; }
    public bool? IsOnFlexiPaymentsPilot { get; set; }
    public string CourseCode { get; set; }
    public IEnumerable<Standard> Standards { get; set; }
}