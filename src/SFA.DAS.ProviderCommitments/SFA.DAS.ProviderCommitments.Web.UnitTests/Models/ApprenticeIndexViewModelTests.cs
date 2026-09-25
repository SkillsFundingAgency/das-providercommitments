using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models;

[TestFixture]
public class ApprenticeIndexViewModelTests
{
    [Test]
    [MoqInlineAutoData(true, "das-table__sort das-table__sort--desc")]
    [MoqInlineAutoData(false, "das-table__sort das-table__sort--asc")]
    public void ThenTheSortByHeaderClassNameIsSetCorrectly(
        bool isReverse,
        string expected,
        IndexViewModel model)
    {
        //Arrange
        model.SortedByHeaderClassName = "";
        model.FilterModel.ReverseSort = isReverse;

        //Act
        model.SortedByHeader();

        //Assert
        model.SortedByHeaderClassName.Should().Be(expected);
    }

    [Test]
    public void SortedByHeader_ThenDoesNotThrowWhenFilterModelIsNull()
    {
        var model = new IndexViewModel
        {
            FilterModel = null,
            SortedByHeaderClassName = ""
        };

        var act = () => model.SortedByHeader();

        act.Should().NotThrow();
        model.ShowPageLinks.Should().BeFalse();
    }
}