namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class OpenAiConfiguration
    {
        public string ResourceName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
        public string Model { get; set; } = null!;
    }
}