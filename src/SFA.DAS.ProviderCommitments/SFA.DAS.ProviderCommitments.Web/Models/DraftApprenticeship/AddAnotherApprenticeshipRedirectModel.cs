namespace SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship
{
    public class AddAnotherApprenticeshipRedirectModel
    {
        public RedirectTarget RedirectTo { get; set; }
        public Guid CacheKey { get; set; }

        public enum RedirectTarget
        {
            SelectCourse,
            SelectLearner
        }
    }
}
