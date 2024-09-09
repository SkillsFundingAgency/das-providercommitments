using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Helpers;

[TestFixture]
public class RecognisePriorLearningHelperTest
{
    [TestCase("2022-08-01", null, true)]
    [TestCase("2022-07-31", null, false)]
    [TestCase(null, "2022-08-01", true)]
    [TestCase(null, "2022-07-31", false)]
    public void DoesDraftApprenticeshipRequireRpl_Returns_Correct_Result(DateTime? actualStartDate, DateTime? plannedStartDate, bool expectedRequireRplStatus)
    {
        var model = new DraftApprenticeshipViewModel
        {
            StartDate = plannedStartDate == null ? new MonthYearModel("") : new MonthYearModel($"{plannedStartDate.Value.Month}{plannedStartDate.Value.Year}"),
            ActualStartDate = actualStartDate == null ? new DateModel() : new DateModel(actualStartDate.Value)
        };

        var result = RecognisePriorLearningHelper.DoesDraftApprenticeshipRequireRpl(model);

        result.Should().Be(expectedRequireRplStatus);
    }
}