using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Cohort;

public class WhenNewingUpFilterForSelectLearnerRecord
{
    [Test]
    public void Then_MonthNames_Should_Be_Populated()
    {
        var filterModel = new LearnerRecordsFilterModel();

        filterModel.MonthNames[0].Value.Should().BeNull();
        filterModel.MonthNames[0].Key.Should().Be("All");
        filterModel.MonthNames[1].Value.Should().Be(1);
        filterModel.MonthNames[1].Key.Should().Be("January");
        filterModel.MonthNames[2].Value.Should().Be(2);
        filterModel.MonthNames[2].Key.Should().Be("February");
        filterModel.MonthNames[3].Value.Should().Be(3);
        filterModel.MonthNames[3].Key.Should().Be("March");
        filterModel.MonthNames[4].Value.Should().Be(4);
        filterModel.MonthNames[4].Key.Should().Be("April");
        filterModel.MonthNames[5].Value.Should().Be(5);
        filterModel.MonthNames[5].Key.Should().Be("May");
        filterModel.MonthNames[6].Value.Should().Be(6);
        filterModel.MonthNames[6].Key.Should().Be("June");
        filterModel.MonthNames[7].Value.Should().Be(7);
        filterModel.MonthNames[7].Key.Should().Be("July");
        filterModel.MonthNames[8].Value.Should().Be(8);
        filterModel.MonthNames[8].Key.Should().Be("August");
        filterModel.MonthNames[9].Value.Should().Be(9);
        filterModel.MonthNames[9].Key.Should().Be("September");
        filterModel.MonthNames[10].Value.Should().Be(10);
        filterModel.MonthNames[10].Key.Should().Be("October");
        filterModel.MonthNames[11].Value.Should().Be(11);
        filterModel.MonthNames[11].Key.Should().Be("November");
        filterModel.MonthNames[12].Value.Should().Be(12);
        filterModel.MonthNames[12].Key.Should().Be("December");
    }

    [Test]
    public void Then_YearNames_Should_Be_Populated()
    {
        var filterModel = new LearnerRecordsFilterModel();

        filterModel.YearNames[0].Should().Be(2024);
        filterModel.YearNames[1].Should().Be(2025);
        filterModel.YearNames[2].Should().Be(2026);
    }
}