using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetAllChangeHistoryApiRequest(
    long providerId,
    int pageNumber = 1,
    int pageSize = Constants.ChangeHistory.NumberOfChangesPerPage)
    : IGetApiRequest
{
    public long ProviderId { get; } = providerId;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;

    public string GetUrl
    {
        get
        {
            var queryParams = new List<string>
            {
                $"pageNumber={PageNumber}",
                $"pageSize={PageSize}"
            };

            return $"provider/{ProviderId}/apprentices/change-history?{string.Join("&", queryParams)}";
        }
    }
}
