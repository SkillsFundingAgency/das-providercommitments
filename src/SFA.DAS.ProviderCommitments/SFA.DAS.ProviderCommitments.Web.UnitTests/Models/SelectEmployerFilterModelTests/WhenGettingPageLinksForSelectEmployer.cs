using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.SelectEmployerFilterModelTests;

public class WhenGettingPageLinksForSelectEmployer
{
    [Test]
    public void Then_Adds_PageLink_For_Every_Page()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            SearchTerm = "test",
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 3,
            CurrentlySortedByField = "EmployerAccountLegalEntityName",
            ReverseSort = false,
            UseLearnerData = true,
            ProviderId = 12345,
            PageNumber = 1
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        for (var index = 0; index < 3; index++)
        {
            pageLinks[index].Label.Should().Be($"{index + 1}");
            pageLinks[index].AriaLabel.Should().Be($"Page {index + 1}");
            pageLinks[index].RouteData.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "providerId", filterModel.ProviderId.ToString() },
                { nameof(filterModel.UseLearnerData), filterModel.UseLearnerData.ToString() },
                { nameof(filterModel.ReverseSort), filterModel.ReverseSort.ToString() },
                { "SortField", filterModel.CurrentlySortedByField ?? string.Empty },
                { nameof(filterModel.SearchTerm), filterModel.SearchTerm ?? string.Empty },
                { nameof(filterModel.PageNumber), (index + 1).ToString() }
            });
        }
    }

    [Test]
    public void And_PageNumber_1_Then_Sets_Current_On_1()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 3
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        pageLinks[0].IsCurrent.Should().BeTrue();
        pageLinks[1].IsCurrent.Should().BeNull();
        pageLinks[2].IsCurrent.Should().BeNull();
    }

    [Test]
    public void And_PageNumber_3_Then_Sets_Current_On_3()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 3,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 3
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        pageLinks[0].IsCurrent.Should().BeNull();
        pageLinks[1].IsCurrent.Should().BeNull();
        pageLinks[2].IsCurrent.Should().BeTrue();
    }

    [Test]
    public void And_PageNumber_5_Of_7_Then_Sets_Current_On_5()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 5,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 7
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        pageLinks[0].IsCurrent.Should().BeNull();
        pageLinks[1].IsCurrent.Should().BeNull();
        pageLinks[2].IsCurrent.Should().BeTrue();
        pageLinks[3].IsCurrent.Should().BeNull();
        pageLinks[4].IsCurrent.Should().BeNull();
    }

    [Test]
    public void And_3_Pages_Then_Only_3_PageLinks()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 3
        };

        // Act
        var numberedCount = filterModel.PageLinks.Count(link =>
            link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT");

        // Assert
        numberedCount.Should().Be(3);
    }

    [Test]
    public void And_More_Than_5_Pages_Then_Only_5_PageLinks()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 10
        };

        // Act
        var numberedCount = filterModel.PageLinks.Count(link =>
            link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT");

        // Assert
        numberedCount.Should().Be(5);
    }

    [Test]
    public void And_Page_Number_4_Of_5_Then_Links_1_To_5()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 4,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 5
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        pageLinks[0].Label.Should().Be("1");
        pageLinks[1].Label.Should().Be("2");
        pageLinks[2].Label.Should().Be("3");
        pageLinks[3].Label.Should().Be("4");
        pageLinks[4].Label.Should().Be("5");
    }

    [Test]
    public void And_Page_Number_7_Of_10_Then_Links_5_To_9()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 7,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 10
        };

        // Act
        var pageLinks = filterModel.PageLinks
            .Where(link => link.Label.ToUpper() != "PREVIOUS" && link.Label.ToUpper() != "NEXT")
            .ToList();

        // Assert
        pageLinks[0].Label.Should().Be("5");
        pageLinks[1].Label.Should().Be("6");
        pageLinks[2].Label.Should().Be("7");
        pageLinks[3].Label.Should().Be("8");
        pageLinks[4].Label.Should().Be("9");
    }

    [Test]
    public void And_1_Page_Then_No_Next_Or_Previous()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage - 1
        };

        // Act
        var pageLinks = filterModel.PageLinks.ToList();

        // Assert
        pageLinks.Any(link => link.Label.ToUpper() == "PREVIOUS").Should().BeFalse();
        pageLinks.Any(link => link.Label.ToUpper() == "NEXT").Should().BeFalse();
    }

    [Test]
    public void And_Not_Last_Page_Then_Adds_Next()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 6
        };

        // Act
        var pageLinks = filterModel.PageLinks.ToList();

        // Assert
        pageLinks.Last().Label.Should().Be("Next");
        pageLinks.Last().AriaLabel.Should().Be("Next page");
        pageLinks.Last().RouteData.Should()
            .BeEquivalentTo(pageLinks.Single(link => link.Label == (filterModel.PageNumber + 1).ToString()).RouteData);
    }

    [Test]
    public void And_Is_Last_Page_Then_No_Next()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 6,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 6
        };

        // Act
        var pageLinks = filterModel.PageLinks.ToList();

        // Assert
        pageLinks.Last().Label.Should().Be("6");
    }

    [Test]
    public void And_Not_First_Page_Then_Adds_Previous()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 2,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 6
        };

        // Act
        var pageLinks = filterModel.PageLinks.ToList();

        // Assert
        pageLinks.First().Label.Should().Be("Previous");
        pageLinks.First().AriaLabel.Should().Be("Previous page");
        pageLinks.First().RouteData.Should()
            .BeEquivalentTo(pageLinks.Single(link => link.Label == "1").RouteData);
    }

    [Test]
    public void And_Is_First_Page_Then_No_Previous()
    {
        // Arrange
        var filterModel = new SelectEmployerFilterModel
        {
            PageNumber = 1,
            TotalEmployersFound = Constants.SelectEmployer.NumberOfEmployersPerPage * 6
        };

        // Act
        var pageLinks = filterModel.PageLinks.ToList();

        // Assert
        pageLinks.First().Label.Should().Be("1");
    }
}
