namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.SelectMultiple;

public class GetSelectMultipleAvailableReservationsRequest(long accountId, long accountLegalEntityId) : IGetApiRequest
{
    public string GetUrl => $"SelectMultiple/AvailableReservations/{accountId}/{accountLegalEntityId}";
}
