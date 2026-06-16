using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class ChangeHistoryTypeExtension
{
    public static string GetDisplayClass(this LearningChangeType changeType)
    {
        return changeType switch
        {
            LearningChangeType.AutoApproved => "govuk-tag--grey",
            LearningChangeType.Rejected or LearningChangeType.EmployerRejected => "govuk-tag--red",
            LearningChangeType.EmployerApproved or LearningChangeType.ManualUpdate => "govuk-tag--green",
            _ => string.Empty,
        };
    }
}