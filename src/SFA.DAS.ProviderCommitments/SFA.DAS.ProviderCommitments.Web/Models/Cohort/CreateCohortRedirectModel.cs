namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class CreateCohortRedirectModel
    {
        public RedirectTarget RedirectTo { get; set; }
        public Guid CacheKey { get; set; }

        public enum RedirectTarget
        {
            SelectCourse,
            ChooseFlexiPaymentPilotStatus
        }
    }
}
