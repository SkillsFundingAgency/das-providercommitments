namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class BeforeYouContinueMultiSelectViewModel
{
    public long ProviderId { get; set; }
    public bool HasCreateCohortPermission { get; set; }
    public bool HasNoDeclaredStandards { get; set; }
}