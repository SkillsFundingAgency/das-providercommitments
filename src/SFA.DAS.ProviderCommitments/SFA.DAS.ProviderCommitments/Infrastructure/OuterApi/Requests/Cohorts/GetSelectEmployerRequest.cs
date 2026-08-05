using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;

public class GetSelectEmployerRequest(
    long providerId,
    string searchTerm,
    string sortField,
    bool reverseSort,    
    int pageNumber = 1,
    int pageSize = 50)
    : GetSelectEmployersRequest(providerId, searchTerm, sortField, reverseSort, pageNumber, pageSize)
    , IGetApiRequest
{

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

            queryParams.Add($"reverseSort={ReverseSort}");
            queryParams.Add($"pageNumber={PageNumber}");
            queryParams.Add($"pageSize={PageSize}");

            return $"provider/{ProviderId}/unapproved/add/select-employer?{string.Join("&", queryParams)}";
        }
    }
}