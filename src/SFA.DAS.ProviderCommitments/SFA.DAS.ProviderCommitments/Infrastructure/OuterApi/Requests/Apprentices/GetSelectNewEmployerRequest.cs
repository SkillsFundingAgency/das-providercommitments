using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetSelectNewEmployerRequest(long apprenticeshipId, long providerId, string searchTerm, string sortField, bool reverseSort, int pageNumber = 1, int pageSize = 50) :
    GetSelectEmployersRequest(providerId, searchTerm, sortField, reverseSort, pageNumber, pageSize), IGetApiRequest
{
    public long ApprenticeshipId { get; set; } = apprenticeshipId;

    public string GetUrl
    {
        get
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                queryParams.Add($"searchTerm={WebUtility.UrlEncode(SearchTerm)}");
            }

            if (!string.IsNullOrWhiteSpace(SortField))
            {
                queryParams.Add($"sortField={WebUtility.UrlEncode(SortField)}");
            }
            queryParams.Add($"apprenticeshipId={ApprenticeshipId}");
            queryParams.Add($"reverseSort={ReverseSort}");
            queryParams.Add($"pageNumber={PageNumber}");
            queryParams.Add($"pageSize={PageSize}");

            return $"provider/{ProviderId}/apprentices/select-employer?{string.Join("&", queryParams)}";
        }
    }
}