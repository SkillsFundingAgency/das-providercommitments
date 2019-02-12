namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class AuthenticationSettings : IAuthenticationSettings
    {
        public string MetadataAddress { get; set; }
        public string Wtrealm { get; set; }
    }
}
