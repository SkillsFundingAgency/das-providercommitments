using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class AlertDisplayExtensions
{
    public const string IlrChangeInvalidText = "ILR change invalid";

    public static string ToAlertDisplayText(this Alerts alert)
    {
        return alert.GetDescription();
    }

    public static bool IsIlrChangeInvalid(this string alertText)
    {
        return alertText == IlrChangeInvalidText;
    }
}
