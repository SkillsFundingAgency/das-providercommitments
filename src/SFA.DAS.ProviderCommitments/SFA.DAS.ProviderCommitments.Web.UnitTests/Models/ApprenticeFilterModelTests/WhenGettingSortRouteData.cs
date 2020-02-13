using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.ApprenticeFilterModelTests
{
    [TestFixture]
    public class WhenGettingSortRouteData
    {
        [Test, AutoData]
        public void Then_Should_Set_SortField(
            string sortField,
            ApprenticesFilterModel model)
        {
            //Act
            var actual = model.BuildSortRouteData(sortField);

            //Assert
            Assert.AreEqual(sortField, actual[nameof(model.SortField)]);
        }

        [Test, AutoData]
        public void And_No_SortField_Then_Should_Set_SortField(
            string sortField,
            ApprenticesFilterModel model)
        {
            //Arrange
            model.SortField = null;

            //Act
            var actual = model.BuildSortRouteData(sortField);

            //Assert
            Assert.AreEqual(sortField, actual[nameof(model.SortField)]);
        }

        [Test, AutoData]
        public void Then_Should_Set_ReverseSort(
            string sortField,
            ApprenticesFilterModel model)
        {
            //Arrange
            model.SortField = sortField;
            var expected = !model.ReverseSort;

            //Act
            var actual = model.BuildSortRouteData(sortField);

            //Assert
            Assert.AreEqual(expected.ToString(), actual[nameof(model.ReverseSort)]);
        }

        [Test, AutoData]
        public void And_No_SortField_Then_Should_Set_ReverseSort(
            string sortField,
            ApprenticesFilterModel model)
        {
            //Arrange
            model.SortField = null;
            var expected = !model.ReverseSort;

            //Act
            var actual = model.BuildSortRouteData(sortField);

            //Assert
            Assert.AreEqual(expected.ToString(), actual[nameof(model.ReverseSort)]);
        }
    }
}