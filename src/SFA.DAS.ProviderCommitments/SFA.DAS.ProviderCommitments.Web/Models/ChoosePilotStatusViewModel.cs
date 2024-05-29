using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models;

public class ChoosePilotStatusViewModel : IAuthorizationContextModel
{
    public Guid CacheKey { get; set; }
    public long ProviderId { get; set; }
    public string CohortReference { get; set; }
    public string DraftApprenticeshipHashedId { get; set; }
    public string ApprenticeshipHashedId { get; set; }
    public long CohortId { get; set; }
    public Guid? ReservationId { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public long AccountLegalEntityId { get; set; }
    public TrainingProgramme[] Courses { get; set; }
    public string CourseCode { get; set; }
    public string StartMonthYear { get; set; }
    public DeliveryModel? DeliveryModel { get; set; }
    public bool ShowTrainingDetails { get; set; }
    public ChoosePilotStatusOptions? Selection { get; set; }
}

public enum ChoosePilotStatusOptions
{
    Pilot,
    NonPilot
}