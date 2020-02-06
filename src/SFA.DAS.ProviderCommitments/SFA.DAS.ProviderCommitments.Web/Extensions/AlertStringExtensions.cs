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
                    return "ILR Data Mismatch";
                case Alerts.ChangesPending:
                    return "Changes Pending";
                case Alerts.ChangesRequested:
                    return "Changes Requested";
                case Alerts.ChangesForReview:
                    return "Changes For Review";
                default:
                    return "";
            }
        }
    }
}