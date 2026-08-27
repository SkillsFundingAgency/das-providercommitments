using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class AlertDisplayExtensions
{
    public const Alerts IlrChangeInvalid = (Alerts)5;
    public const string IlrChangeInvalidText = "ILR change invalid";
    public const Alerts ChangesDeclined = (Alerts)6;
    public const string ChangesDeclinedText = "Changes declined";

    public static string ToAlertDisplayText(this Alerts alert)
    {
        return (int)alert == (int)IlrChangeInvalid
            ? IlrChangeInvalidText
            : (int)alert == (int)ChangesDeclined
                ? ChangesDeclinedText
                : alert.GetDescription();
    }

    public static bool IsIlrChangeInvalid(this string alertText)
    {
        return alertText == IlrChangeInvalidText;
    }

    public static bool IsChangesDeclined(this string alertText)
    {
        return alertText == ChangesDeclinedText;
    }
}
