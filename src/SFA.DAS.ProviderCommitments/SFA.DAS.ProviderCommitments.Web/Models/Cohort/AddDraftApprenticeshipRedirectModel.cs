namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class AddDraftApprenticeshipRedirectModel
{
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public bool IsEdit { get; set; }

    public long? OverlappingApprenticeshipId { get; set; }
    public RedirectTarget RedirectTo { get; set; }

    public enum RedirectTarget
    {
        SelectCourse,
        SelectDeliveryModel,
        SelectPilotStatus,
        OverlapWarning,
        SaveApprenticeship
    }
}