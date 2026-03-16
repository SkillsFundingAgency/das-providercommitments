using SFA.DAS.ProviderCommitments;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.SelectEmployerFilterModelTests;

public class WhenGettingPagedRecordsTo
{
    [Test]
    public void And_PageNumber_1_Then_Should_Be_PageSize()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsTo;

        // Assert
        actual.Should().Be(Constants.SelectEmployer.NumberOfEmployersPerPage);
    }

    [Test]
    public void And_PageNumber_2_Then_Should_Be_Double_PageSize()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 2,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsTo;

        // Assert
        actual.Should().Be(2 * Constants.SelectEmployer.NumberOfEmployersPerPage);
    }

    [Test]
    public void And_PageNumber_3_Then_Should_Be_Triple_PageSize()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 3,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsTo;

        // Assert
        actual.Should().Be(3 * Constants.SelectEmployer.NumberOfEmployersPerPage);
    }

    [Test]
    public void And_TotalRecords_Less_Than_Calculated_PagedRecordsTo_Then_Is_TotalRecords()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 3,
            TotalEmployersFound = 3 * Constants.SelectEmployer.NumberOfEmployersPerPage - 20
        };

        // Act
        var actual = filterModel.PagedRecordsTo;

        // Assert
        actual.Should().Be(filterModel.TotalEmployersFound);
    }
}
