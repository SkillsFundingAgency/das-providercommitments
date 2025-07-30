namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectDraftApprenticeshipsEntryMethodViewModel
{
    public long ProviderId { get; set; }

    public AddDraftApprenticeshipEntryMethodOptions? Selection { get; set; }
}

public enum AddDraftApprenticeshipEntryMethodOptions
{
    BulkCsv,
    ILR
}