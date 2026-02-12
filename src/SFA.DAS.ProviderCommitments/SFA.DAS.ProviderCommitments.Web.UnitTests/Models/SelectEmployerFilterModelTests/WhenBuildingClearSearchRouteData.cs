using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.SelectEmployerFilterModelTests;

public class WhenBuildingClearSearchRouteData
{
    [Test]
    public void Then_Returns_Route_Data_With_ProviderId_UseLearnerData_And_Reset_Values()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            ProviderId = 12345,
            UseLearnerData = true,
            SearchTerm = "test",
            PageNumber = 3,
            CurrentlySortedByField = "EmployerAccountLegalEntityName",
            ReverseSort = true
        };

        // Act
        var routeData = filterModel.BuildClearSearchRouteData();

        // Assert
        routeData.Should().ContainKey("providerId").WhoseValue.Should().Be("12345");
        routeData.Should().ContainKey("UseLearnerData").WhoseValue.Should().Be("True");
        routeData.Should().ContainKey("ReverseSort").WhoseValue.Should().Be("False");
        routeData.Should().ContainKey("SortField").WhoseValue.Should().Be(string.Empty);
        routeData.Should().ContainKey("SearchTerm").WhoseValue.Should().Be(string.Empty);
        routeData.Should().ContainKey("PageNumber").WhoseValue.Should().Be("1");
    }
}
