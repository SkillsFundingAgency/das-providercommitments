using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Learners;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Learners;

public class SelectMultipleLearnerRecordsViewModelTests
{
    private SelectMultipleLearnerRecordsViewModel _viewModel;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
        _viewModel = _fixture.Create<SelectMultipleLearnerRecordsViewModel>();
    }

    [TestCase(true, "das-table__sort--desc")]
    [TestCase(false, "das-table__sort--asc")]
    public void SortedByHeaderClassNameIsCorrect(bool descending, string expected)
    {
        // Arrange
        _viewModel.FilterModel.ReverseSort = descending;

        // Act
        _viewModel.SortedByHeader();

        //Assert
        _viewModel.SortedByHeaderClassName.Should().Contain(expected);
    }

    [Test]
    public void LastSubmittedOnIsNull_DescIsBlank()
    {
        _viewModel.LastIlrSubmittedOn = null;
        _viewModel.LastIlrSubmittedOnDesc.Should().BeEmpty();
    }

    [Test]
    public void LastSubmittedOnIsNotBlank_DescIsCorrect()
    {
        _viewModel.LastIlrSubmittedOn = new DateTime(2025, 4, 10);
        _viewModel.LastIlrSubmittedOnDesc.Should().Be("Last updated 1:00AM on Thursday 10 April");
    }

    [TestCase(1, "1 apprentice records")]
    [TestCase(100, "100 apprentice records")]
    public void TotalNumberOfApprenticesDescription_IsCorrect(int count, string expected)
    {
        _viewModel.FilterModel.TotalNumberOfLearnersFound = count;
        _viewModel.FilterModel.TotalNumberOfApprenticeshipsFoundDescription.ToString().Should().StartWith(expected);
    }

    [Test]
    public void GettingFiltersUsed_IsJustYearWhenBlank()
    {
        _viewModel.FilterModel.SearchTerm = "";
        _viewModel.FilterModel.StartMonth = "";
        _viewModel.FilterModel.StartYear = "2025";
        _viewModel.FilterModel.TotalNumberOfApprenticeshipsFoundDescription.ToString().Should().EndWith("<strong>2025</strong>");
    }

    [TestCase("", "", "<strong>2025</strong>")]
    [TestCase("XXX", "", "<strong>‘XXX’</strong> and <strong>2025</strong>")]
    [TestCase("XXX", "1", "<strong>‘XXX’</strong>, <strong>January</strong> and <strong>2025</strong>")]
    [TestCase(null, "5", "<strong>May</strong> and <strong>2025</strong>")]
    public void GettingFiltersUsed_MatchesExpected(string searchTerm, string startMonth, string expected)
    {
        _viewModel.FilterModel = new MultipleLearnerRecordsFilterModel();
        _viewModel.FilterModel.SearchTerm = searchTerm;
        _viewModel.FilterModel.StartMonth = startMonth;
        _viewModel.FilterModel.StartYear = "2025";
        _viewModel.FilterModel.TotalNumberOfApprenticeshipsFoundDescription.ToString().Should().EndWith(expected);
    }

    [TestCase("", 2)]
    [TestCase(null, 2)]
    public void BuildRouteData_IsCorrect(string searchTerm, int expectedCount)
    {
        _viewModel.FilterModel.SearchTerm = searchTerm;

        var routeData = _viewModel.FilterModel.RouteData;

        routeData.Should().NotBeNull();
        routeData.Count.Should().Be(expectedCount);
        routeData["ProviderId"].Should().Be(_viewModel.FilterModel.ProviderId.ToString());
        routeData["CacheKey"].Should().Be(_viewModel.FilterModel.CacheKey.ToString());
    }

    [Test]
    public void MapsIlrLearnerSummary_ToIlrApprenticeshipSummary()
    {
        var ilrLearner = _fixture.Create<GetLearnerSummary>();
        var apprenticeship = (LearnerSummary)ilrLearner;

        apprenticeship.Should().NotBeNull();
        apprenticeship.Id.Should().Be(ilrLearner.Id);
        apprenticeship.FirstName.Should().Be(ilrLearner.FirstName);
        apprenticeship.LastName.Should().Be(ilrLearner.LastName);
        apprenticeship.Uln.Should().Be(ilrLearner.Uln);
        apprenticeship.CourseName.Should().Be(ilrLearner.Course);
    }

    [TestCase(0, false)]
    [TestCase(3, true)]
    public void IsNonLevy_Calculation(int futureMonths, bool expected)
    {
        _viewModel.FutureMonths = futureMonths;

        var isNonLevy = _viewModel.IsNonLevy;
        isNonLevy.Should().Be(expected);
    }
}