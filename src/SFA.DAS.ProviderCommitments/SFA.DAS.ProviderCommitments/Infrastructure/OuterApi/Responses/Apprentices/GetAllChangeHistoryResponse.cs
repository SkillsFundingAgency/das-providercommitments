using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class GetAllChangeHistoryResponse
{
    public List<GetChangeHistoryItem> ChangeHistory { get; set; } = new List<GetChangeHistoryItem>();
    public int TotalCount { get; set; }
    public DateTime? AvailableFrom { get; set; }
}