using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Helpers;

public static class RecognisePriorLearningHelper
{
    public static bool DoesDraftApprenticeshipRequireRpl(DraftApprenticeshipViewModel model)
    {
        return DoesDraftApprenticeshipRequireRpl(model.ActualStartDate.Date, model.StartDate.Date);
    }

    public static bool DoesDraftApprenticeshipRequireRpl(DateTime? actualStartDate, DateTime? plannedStartDate)
    {
        var startDate = actualStartDate ?? plannedStartDate;
        return startDate?.Date >= new DateTime(2022, 08, 01);
    }
}