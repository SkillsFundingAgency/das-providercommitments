namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class ValidateEditApprenticeshipResponse
{
    public long ApprenticeshipId { get; set; }
    public bool HasOptions { get; set; }
    public bool NeedReapproval { get; set; }
}