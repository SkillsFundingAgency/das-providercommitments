using SFA.DAS.ProviderCommitments;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.SelectEmployerFilterModelTests;

public class WhenGettingPagedRecordsFrom
{
    [Test]
    public void And_PageNumber_1_Then_Should_Be_1()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsFrom;

        // Assert
        actual.Should().Be(1);
    }

    [Test]
    public void And_PageNumber_2_Then_Should_Be_PageSize_Plus_1()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 2,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsFrom;

        // Assert
        actual.Should().Be(Constants.SelectEmployer.NumberOfEmployersPerPage + 1);
    }

    [Test]
    public void And_PageNumber_3_Then_Should_Be_Double_PageSize_Plus_1()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 3,
            TotalEmployersFound = 20 * Constants.SelectEmployer.NumberOfEmployersPerPage
        };

        // Act
        var actual = filterModel.PagedRecordsFrom;

        // Assert
        actual.Should().Be(2 * Constants.SelectEmployer.NumberOfEmployersPerPage + 1);
    }

    [Test]
    public void And_PageNumber_1_And_0_Records_Found_Then_Should_Be_0()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = 0
        };

        // Act
        var actual = filterModel.PagedRecordsFrom;

        // Assert
        actual.Should().Be(0);
    }
}
