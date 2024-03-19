using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

public sealed class GetHasPermissionResponse
{
    [JsonPropertyName(nameof(HasPermission))]
    public bool HasPermission { get; set; }
}