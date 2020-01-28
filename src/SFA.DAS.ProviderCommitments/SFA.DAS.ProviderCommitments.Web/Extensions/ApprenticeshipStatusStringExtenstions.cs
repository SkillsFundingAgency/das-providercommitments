using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ApprenticeshipStatusStringExtenstions
    {
        public static string FormatStatus(this ApprenticeshipStatus status)
        {
            if (status == ApprenticeshipStatus.WaitingToStart)
                return "Waiting To Start";
            return status.ToString();
        }
    }
}