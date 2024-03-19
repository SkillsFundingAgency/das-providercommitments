using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class SelectPilotStatusViewModel : IAuthorizationContextModel
{
    public Guid CacheKey { get; set; }
    public long ProviderId { get; set; }
    public ChoosePilotStatusOptions? Selection { get; set; }
    public bool IsEdit { get; set; }
}