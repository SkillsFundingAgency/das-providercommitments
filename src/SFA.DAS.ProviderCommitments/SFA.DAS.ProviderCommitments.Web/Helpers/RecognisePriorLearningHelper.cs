using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Helpers
{
    public static class RecognisePriorLearningHelper
    {
        public static bool DoesDraftApprenticeshipRequireRpl(DraftApprenticeshipViewModel model)
        {
            var startDate = model.ActualStartDate.Date ?? model.StartDate.Date;
            return startDate?.Date >= new DateTime(2022, 08, 01);
        }
    }
}