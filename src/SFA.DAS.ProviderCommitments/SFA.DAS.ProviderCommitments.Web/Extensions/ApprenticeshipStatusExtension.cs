using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ApprenticeshipStatusExtension
    {
        public static string FormatStatus(this ApprenticeshipStatus status)
        {
            switch (status)
            {
                case ApprenticeshipStatus.WaitingToStart : return "Waiting To Start";
                case ApprenticeshipStatus.Live : return "Live";
                case ApprenticeshipStatus.Paused : return "Paused";
                case ApprenticeshipStatus.Completed : return "Completed";
                case ApprenticeshipStatus.Stopped : return "Stopped";
               
                default: return "";
            }
        }
    }
}
