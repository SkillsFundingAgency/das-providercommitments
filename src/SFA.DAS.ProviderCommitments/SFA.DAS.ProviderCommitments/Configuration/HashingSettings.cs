namespace SFA.DAS.ProviderCommitments.Configuration
{
    public class HashingConfiguration
    {
        public string Alphabet { get; set; }
        public string Salt { get; set; }
    }

    public class PublicAccountIdHashingConfiguration : HashingConfiguration
    {

    }

    public class PublicAccountLegalEntityIdHashingConfiguration : HashingConfiguration
    {

    }
}