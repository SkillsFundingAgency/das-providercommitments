using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Apprentice;

public class GetAllChangeHistoryListViewModelTests
{
    [TestCase(0, 0, 0)]
    [TestCase(50, 1, 50)]
    [TestCase(51, 1, 50)]
    public void Then_PagedRecordsFrom_And_To_Are_Calculated_For_First_Page(int totalCount, int expectedFrom, int expectedTo)
    {
        var viewModel = new GetAllChangeHistoryListViewModel { TotalCount = totalCount, PageNumber = 1 };

        viewModel.PagedRecordsFrom.Should().Be(expectedFrom);
        viewModel.PagedRecordsTo.Should().Be(expectedTo);
    }

    [TestCase(0, false)]
    [TestCase(50, false)]
    [TestCase(51, true)]
    public void Then_ShowPageLinks_Is_Set_Only_When_Records_Exceed_Page_Size(int totalCount, bool expected)
    {
        var viewModel = new GetAllChangeHistoryListViewModel { TotalCount = totalCount, PageNumber = 1 };

        viewModel.ShowPageLinks.Should().Be(expected);
    }

    [Test]
    public void Then_Second_Page_Shows_Remaining_Records()
    {
        var viewModel = new GetAllChangeHistoryListViewModel { TotalCount = 51, PageNumber = 2 };

        viewModel.PagedRecordsFrom.Should().Be(51);
        viewModel.PagedRecordsTo.Should().Be(51);
    }

    [TestCase(0, 0)]
    [TestCase(50, 1)]
    [TestCase(51, 2)]
    [TestCase(120, 3)]
    public void Then_TotalPages_Is_Calculated(int totalCount, int expected)
    {
        var viewModel = new GetAllChangeHistoryListViewModel { TotalCount = totalCount };

        viewModel.TotalPages.Should().Be(expected);
    }

    [TestCase(1, 0, false)]
    [TestCase(5, 0, false)]
    [TestCase(1, 50, false)]
    [TestCase(2, 50, true)]
    [TestCase(2, 51, false)]
    [TestCase(3, 120, false)]
    [TestCase(5, 120, true)]
    public void Then_IsPageOutOfRange_Is_Calculated(int pageNumber, int totalCount, bool expected)
    {
        var viewModel = new GetAllChangeHistoryListViewModel { PageNumber = pageNumber, TotalCount = totalCount };

        viewModel.IsPageOutOfRange.Should().Be(expected);
    }
}