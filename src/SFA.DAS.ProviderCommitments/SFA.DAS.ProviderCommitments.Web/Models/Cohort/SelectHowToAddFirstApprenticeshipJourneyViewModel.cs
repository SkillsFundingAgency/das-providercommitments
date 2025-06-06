using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectHowToAddFirstApprenticeshipJourneyViewModel : IAuthorizationContextModel
{
    [FromRoute]
    public long ProviderId { get; set; }
    public Guid CacheKey { get; set; }
    public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    public AddFirstDraftApprenticeshipJourneyOptions? Selection { get; set; }
}

public enum AddFirstDraftApprenticeshipJourneyOptions
{
    Ilr,
    Manual
}