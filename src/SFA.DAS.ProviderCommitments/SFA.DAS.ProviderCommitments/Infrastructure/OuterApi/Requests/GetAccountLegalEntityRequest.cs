namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetAccountLegalEntityRequest : IGetApiRequest
    {
        public long _accountLegalEntityId;
        public string GetUrl => $"accountLegalEntity/{_accountLegalEntityId}";

        public GetAccountLegalEntityRequest(long accountLegalEntityId)
        {
            _accountLegalEntityId = accountLegalEntityId;
        }
    }
}
