using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class DeleteCohortViewModel : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public long CohortId { get; set; }
        public string EmployerAccountName { get; set; }
        public int NumberOfApprenticeships { get; set; }
        public List<string> ApprenticeshipTrainingProgrammes { get; set; }
        public bool? Confirm { get; set; }
    }
}
