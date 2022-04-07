namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class GetAccountLegalEntityRequest : IGetApiRequest
    {
        private readonly long _accountLegalEntityId;
        public string GetUrl => $"accountLegalEntity/{_accountLegalEntityId}";

        public GetAccountLegalEntityRequest(long accountLegalEntityId)
        {
            _accountLegalEntityId = accountLegalEntityId;
        }
    }
}
