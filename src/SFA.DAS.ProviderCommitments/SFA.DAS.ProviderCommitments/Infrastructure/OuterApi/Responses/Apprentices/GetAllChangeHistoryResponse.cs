using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class GetAllChangeHistoryResponse
{
    public List<GetChangeHistoryItem> ChangeHistory { get; set; } = new List<GetChangeHistoryItem>();
}