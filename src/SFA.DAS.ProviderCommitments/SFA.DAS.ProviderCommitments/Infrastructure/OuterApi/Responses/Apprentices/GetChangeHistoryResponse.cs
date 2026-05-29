using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class GetChangeHistoryResponse
{
    public IEnumerable<GetChangeHistoryItem> ChangeHistory { get; set; }
}

public class GetChangeHistoryItem
{
    public byte ChangeType { get; set; }
    public string Description { get; set; }
    public long ApprenticeshipId { get; set; }
    public string LearnerName { get; set; }
    public DateTime AppliedDate { get; set; }
    public Guid Id { get; set; }
}