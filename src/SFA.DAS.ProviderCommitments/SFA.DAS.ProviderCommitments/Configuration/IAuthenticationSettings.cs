namespace SFA.DAS.ProviderCommitments.Configuration
{
    public interface IAuthenticationSettings
    {
        string MetadataAddress { get; set; }
        string Wtrealm { get; set; }
    }
}
