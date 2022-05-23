namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class GetAccountLegalEntityQueryResult
    {
        public long AccountId { get; set; }
        public long MaLegalEntityId { get; set; }
        public string AccountName { get; set; }
        public string LegalEntityName { get; set; }
        public ApprenticeshipEmployerType LevyStatus { get; set; }
    }

    public enum ApprenticeshipEmployerType : byte
    {
        NonLevy = 0,
        Levy = 1
    }
}
