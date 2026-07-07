namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetApprenticeshipsFiltersRequest : IGetApiRequest
{
    public long? EmployerAccountId { get; set; }

    public long? ProviderId { get; set; }

    public string GetUrl => $"provider/{ProviderId}/apprenticeships/filters";
}