using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Authorization;

public record OperationPermission
{
    public long? AccountLegalEntityId { get; set; }
    public Operation Operation { get; set; }
    public bool HasPermission { get; set; }
}