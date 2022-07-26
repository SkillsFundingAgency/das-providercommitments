namespace SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate
{
    public class EmployerNotifiedViewModel
    {
        public long ProviderId { get; set; }
        public string CohortReference { get; set; }
        public NextAction? NextAction { get; set; }
    }

    public enum NextAction
    {
        ViewAllCohorts,
        AddAddprentice,
        ViewDashBoard
    }
}
