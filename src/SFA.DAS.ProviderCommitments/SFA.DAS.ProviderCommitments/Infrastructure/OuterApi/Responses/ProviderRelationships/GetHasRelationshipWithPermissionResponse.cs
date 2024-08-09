using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
public class GetHasRelationshipWithPermissionResponse
{
    [JsonPropertyName(nameof(HasPermission))]
    public bool HasPermission { get; set; }
}
