using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class CohortSummaryExtension
    {
        public static CohortStatus GetStatus(this CohortSummary cohort)
        {
            if (cohort.IsDraft && cohort.WithParty == Party.Provider)
                return CohortStatus.Draft;
            
            if (!cohort.IsDraft && cohort.WithParty == Party.Provider)
                return CohortStatus.Review;
            
            if (!cohort.IsDraft && cohort.WithParty == Party.Employer)
                return CohortStatus.WithEmployer;
            
            if (!cohort.IsDraft && cohort.WithParty == Party.TransferSender)
                return CohortStatus.WithTransferSender;
            
            return CohortStatus.Unknown;
        }
    }

    public enum CohortStatus
    {
        Unknown,
        Draft,
        Review,
        WithEmployer,
        WithTransferSender
    }
}
