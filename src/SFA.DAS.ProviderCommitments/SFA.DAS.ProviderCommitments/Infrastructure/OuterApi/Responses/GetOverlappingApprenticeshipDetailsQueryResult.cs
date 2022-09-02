namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    public class ValidateUlnOverlapOnStartDateQueryResult
    {
        public long? HasOverlapWithApprenticeshipId { get; set; }
        public bool HasStartDateOverlap { get; set; }
    }
}
