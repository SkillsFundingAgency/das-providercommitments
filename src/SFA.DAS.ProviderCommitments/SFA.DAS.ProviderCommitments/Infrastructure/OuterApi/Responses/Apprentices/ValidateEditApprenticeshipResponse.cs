namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;

public class ValidateEditApprenticeshipResponse
{
    public long ApprenticeshipId { get; set; }
    public bool HasOptions { get; set; }
    public bool CourseOrStartDateChange { get; set; }
    public string Version { get; set; }
}