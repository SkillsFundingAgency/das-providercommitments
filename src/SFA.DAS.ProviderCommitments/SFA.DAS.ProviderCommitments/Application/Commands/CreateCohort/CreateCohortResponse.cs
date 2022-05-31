namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortResponse
    {
        public long CohortId { get; set; }
        public string CohortReference { get; set; }
        public bool HasStandardOptions { get; set; }
        public long? DraftApprenticeshipId { get ; set ; }
    }
}
