namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetAccountLegalEntityRequest : IGetApiRequest
    {
        private long _accountLegalEntityId { get; }
        public string GetUrl => $"accountLegalEntity/{_accountLegalEntityId}";

        public GetAccountLegalEntityRequest(long accountLegalEntityId)
        {
            _accountLegalEntityId = accountLegalEntityId;
        }
    }
}
