namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class ConfirmEditApprenticeshipResponse
{
    public long ApprenticeshipId { get; set; }
    public bool NeedReapproval { get; set; }
}