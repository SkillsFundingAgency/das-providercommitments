using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models
{
    [TestFixture]
    public class ManageApprenticesModelTests
    {
        //TODO to move
        [Test]
        [MoqInlineAutoData(true, "das-table__sort das-table__sort--desc")]
        [MoqInlineAutoData(false, "das-table__sort das-table__sort--asc")]
        public void ThenTheSortByHeaderClassNameIsSetCorrectly(
            bool isReverse,
            string expected,
            ManageApprenticesViewModel model)
        {
            //Arrange
            model.SortedByHeaderClassName = "";
            model.FilterModel.ReverseSort = isReverse;

            //Act
            model.SortedByHeader();

            //Assert
            Assert.AreEqual(expected, model.SortedByHeaderClassName);
        }
    }
}