using System;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetInvalidIlrChangesRequest(long providerId, long apprenticeshipId) : IGetApiRequest
{
    public string GetUrl => $"{providerId}/apprentices/{apprenticeshipId}/invalid-ilr-changes";
}

public class PostInvalidIlrChangesRequest(long providerId, long apprenticeshipId, PostInvalidIlrChangesRequestData data) : IPostApiRequest
{
    public string PostUrl => $"{providerId}/apprentices/{apprenticeshipId}/invalid-ilr-changes";
    public object Data { get; set; } = data;
}

public class PostInvalidIlrChangesRequestData : ApimSaveDataRequest
{
    public List<InvalidIlrChangeAcknowledgement> Acknowledgements { get; set; } = [];
}

public class InvalidIlrChangeAcknowledgement
{
    public Guid ApprovalRequestId { get; set; }
    public bool? DeleteAlert { get; set; }
}

public class GetInvalidIlrChangesResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<InvalidIlrChangeSet> RequestSets { get; set; } = [];
}

public class InvalidIlrChangeSet
{
    public Guid ApprovalRequestId { get; set; }
    public string Decision { get; set; }
    public List<InvalidIlrChangeField> Fields { get; set; } = [];
}

public class InvalidIlrChangeField
{
    public string Field { get; set; }
    public string Old { get; set; }
    public string New { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public string Reason { get; set; }
}
