using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class GetAllChangeHistoryRequest : IAuthorizationContextModel
{
    [FromRoute]
    public long ProviderId { get; set; }
}