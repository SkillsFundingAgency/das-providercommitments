using System;
using System.Collections.Generic;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetInvalidIlrChangesRequest(long providerId, long apprenticeshipId, string path = "invalid-ilr-changes") : IGetApiRequest
{
    public const string InvalidIlrChangesPath = "invalid-ilr-changes";
    public const string DeclinedChangesPath = "declined-changes";

    public string GetUrl => $"provider/{providerId}/apprentices/{apprenticeshipId}/{path}";
}

public class PostInvalidIlrChangesRequest(long providerId, long apprenticeshipId, PostInvalidIlrChangesRequestData data, string path = "invalid-ilr-changes") : IPostApiRequest
{
    public string PostUrl => $"provider/{providerId}/apprentices/{apprenticeshipId}/{path}";
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
