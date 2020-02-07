using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class AlertStringExtension
    {
        public static string FormatAlert(this Alerts alert)
        {
            switch (alert)
            {
                case Alerts.IlrDataMismatch:
                    return "ILR data mismatch";
                case Alerts.ChangesPending:
                    return "Changes pending";
                case Alerts.ChangesRequested:
                    return "Changes requested";
                case Alerts.ChangesForReview:
                    return "Changes for review";
                default:
                    return "";
            }
        }
    }
}