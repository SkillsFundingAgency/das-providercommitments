using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Ilr;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Cohort;

public class SelectLearnerRecordViewModelTests
{
    private SelectLearnerRecordViewModel _viewModel;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
        _viewModel  = _fixture.Create<SelectLearnerRecordViewModel>();
    }

    [TestCase("Test Name")]
    public void PageTitleIsCorrect(string employerName)
    {
        _viewModel.EmployerAccountName = employerName;
        //Assert
        _viewModel.PageTitle.Should().EndWith(employerName);
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
        _viewModel.LastIlrSubmittedOn = new DateTime(2025,4,10);
        _viewModel.LastIlrSubmittedOnDesc.Should().Be("Last updated 12:00AM on Thursday 10 April");
    }

    [TestCase(1, "1 apprentice record")]
    [TestCase(100, "100 apprentice records")]
    public void TotalNumberOfApprenticesDescription_IsCorrect(int count, string expected)
    {
        _viewModel.FilterModel.TotalNumberOfLearnersFound = count;
        _viewModel.FilterModel.TotalNumberOfApprenticeshipsFoundDescription.Should().Be(expected);
    }

    [TestCase("field1", true)]
    [TestCase("field1", false)]
    public void BuildSortRouteData_IsAlwayAscendingWhenNewField(string field, bool reverse)
    {
        _viewModel.FilterModel.SortField = "";

        var sort = _viewModel.FilterModel.BuildSortRouteData(field);
        sort["SortField"].Should().Be(field);
        sort["ReverseSort"].Should().Be("False");
    }

    [TestCase("field1", true)]
    [TestCase("field1", false)]
    public void BuildSortRouteData_IsAlwayReversingSortWhenSameField(string field, bool reverse)
    {
        _viewModel.FilterModel.SortField = field;
        _viewModel.FilterModel.ReverseSort = reverse;

        var sort = _viewModel.FilterModel.BuildSortRouteData(field);
        sort["SortField"].Should().Be(field);
        sort["ReverseSort"].Should().Be((!reverse).ToString());
    }

    [TestCase("", 3)]
    [TestCase(null, 3)]
    public void BuildRouteData_IsCorrect(string searchTerm, int expectedCount)
    {
        _viewModel.FilterModel.SearchTerm = searchTerm;

        var routeData = _viewModel.FilterModel.RouteData;

        routeData.Should().NotBeNull();
        routeData.Count.Should().Be(expectedCount);
        routeData["ProviderId"].Should().Be(_viewModel.FilterModel.ProviderId.ToString());
        routeData["CacheKey"].Should().Be(_viewModel.FilterModel.CacheKey.ToString());
        routeData["EmployerAccountLegalEntityPublicHashedId"].Should().Be(_viewModel.FilterModel.EmployerAccountLegalEntityPublicHashedId);
    }

    [Test]
    public void BuildRouteDataForSearchTerm_IsCorrect()
    {
        _viewModel.FilterModel.SearchTerm = "test";

        var routeData = _viewModel.FilterModel.RouteData;

        routeData.Should().NotBeNull();
        routeData.Count.Should().Be(4);
        routeData["SearchTerm"].Should().Be(_viewModel.FilterModel.SearchTerm);
    }

    [Test]
    public void MapsIlrLearnerSummary_ToIlrApprenticeshipSummary()
    {
        var ilrLearner = _fixture.Create<GetLearnerSummary>();
        var apprenticeship = (LearnerSummary) ilrLearner;

        apprenticeship.Should().NotBeNull();
        apprenticeship.Id.Should().Be(ilrLearner.Id);
        apprenticeship.FirstName.Should().Be(ilrLearner.FirstName);
        apprenticeship.LastName.Should().Be(ilrLearner.LastName);
        apprenticeship.Uln.Should().Be(ilrLearner.Uln);
        apprenticeship.CourseName.Should().Be(ilrLearner.Course);
    }
}