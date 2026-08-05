namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.FundingOptions;
public class GetAccountFundingOptionsRequest : IGetApiRequest
{
    private readonly long _accountId;
    public string GetUrl => $"{_accountId}/unapproved/add/select-funding";

    public GetAccountFundingOptionsRequest(long accountId)
    {
        _accountId = accountId;
    }
}
