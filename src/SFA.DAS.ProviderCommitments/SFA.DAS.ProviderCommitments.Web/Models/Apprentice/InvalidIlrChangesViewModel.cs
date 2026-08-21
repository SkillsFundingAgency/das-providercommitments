using SFA.DAS.ProviderCommitments.Web.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

public class InvalidIlrChangesRequest : IAuthorizationContextModel
{
    [FromRoute]
    public string ApprenticeshipHashedId { get; set; }

    public long ApprenticeshipId { get; set; }

    [FromRoute]
    public long ProviderId { get; set; }
}

public class InvalidIlrChangesViewModel : IAuthorizationContextModel
{
    public long ProviderId { get; set; }
    public string ApprenticeshipHashedId { get; set; }
    public long ApprenticeshipId { get; set; }
    public string LearnerName { get; set; }
    public List<InvalidIlrChangeSetViewModel> RequestSets { get; set; } = [];
}

public class InvalidIlrChangeSetViewModel
{
    public Guid ApprovalRequestId { get; set; }
    public string Decision { get; set; }
    public bool? DeleteAlert { get; set; }
    public List<InvalidIlrChangeFieldViewModel> Fields { get; set; } = [];
}

public class InvalidIlrChangeFieldViewModel
{
    public string Field { get; set; }
    public string FieldDisplayName { get; set; }
    public string Old { get; set; }
    public string New { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public string Reason { get; set; }
}

public class InvalidIlrChangesAcknowledgementResult
{
}
