using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Learners;

public class WhenNewingUpFilterForSelectLearnerRecord
{
    [Test]
    public void Then_MonthNames_Should_Be_Populated()
    {
        var filterModel = new LearnerRecordsFilterModel();

        filterModel.MonthNames[0].Value.Should().Be("");
        filterModel.MonthNames[0].Text.Should().Be("All");
        filterModel.MonthNames[1].Value.Should().Be("1");
        filterModel.MonthNames[1].Text.Should().Be("January");
        filterModel.MonthNames[2].Value.Should().Be("2");
        filterModel.MonthNames[2].Text.Should().Be("February");
        filterModel.MonthNames[3].Value.Should().Be("3");
        filterModel.MonthNames[3].Text.Should().Be("March");
        filterModel.MonthNames[4].Value.Should().Be("4");
        filterModel.MonthNames[4].Text.Should().Be("April");
        filterModel.MonthNames[5].Value.Should().Be("5");
        filterModel.MonthNames[5].Text.Should().Be("May");
        filterModel.MonthNames[6].Value.Should().Be("6");
        filterModel.MonthNames[6].Text.Should().Be("June");
        filterModel.MonthNames[7].Value.Should().Be("7");
        filterModel.MonthNames[7].Text.Should().Be("July");
        filterModel.MonthNames[8].Value.Should().Be("8");
        filterModel.MonthNames[8].Text.Should().Be("August");
        filterModel.MonthNames[9].Value.Should().Be("9");
        filterModel.MonthNames[9].Text.Should().Be("September");
        filterModel.MonthNames[10].Value.Should().Be("10");
        filterModel.MonthNames[10].Text.Should().Be("October");
        filterModel.MonthNames[11].Value.Should().Be("11");
        filterModel.MonthNames[11].Text.Should().Be("November");
        filterModel.MonthNames[12].Value.Should().Be("12");
        filterModel.MonthNames[12].Text.Should().Be("December");
    }

    [Test]
    public void Then_YearNames_Should_Be_Populated()
    {
        var filterModel = new LearnerRecordsFilterModel();

        filterModel.YearNames[0].Text.Should().Be("2024");
        filterModel.YearNames[0].Value.Should().Be("2024");
        filterModel.YearNames[1].Text.Should().Be("2025");
        filterModel.YearNames[1].Value.Should().Be("2025");
        filterModel.YearNames[2].Text.Should().Be("2026");
        filterModel.YearNames[2].Value.Should().Be("2026");
    }
}