using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships
{
    public class GetHasPermissionResponse
    {
        [JsonPropertyName(nameof(HasPermission))]
        public bool HasPermission { get; set; }
    }
}
