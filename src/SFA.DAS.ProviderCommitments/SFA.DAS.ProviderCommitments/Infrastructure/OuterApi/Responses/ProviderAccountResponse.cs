using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class ProviderAccountResponse
    {
        [JsonPropertyName("canAccessService")]
        public bool CanAccessService { get; set; }
    }
}
