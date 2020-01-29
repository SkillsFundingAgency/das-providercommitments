using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        [Test, MoqAutoData]
        public void IfCalledWithAnInvalidRequestShouldGetBadResponseReturned(
            long providerId,
            string sortField,
            ManageApprenticesFilterModel filterModel,
            [Frozen] Mock<IMapper<Requests.GetApprenticeshipsRequest, ManageApprenticesViewModel>> apprenticeshipMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = controller.Index(providerId, filterModel);

            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test, MoqAutoData]
        public async Task Then_Calls_Mapper_Service_With_Correct_Values(
            long providerId,
            string sortField,
            ManageApprenticesFilterModel filterModel,
            [Frozen] Mock<IMapper<Requests.GetApprenticeshipsRequest, ManageApprenticesViewModel>> mockMapper,
            ManageApprenticesController controller)
        {
            await controller.Index(providerId, filterModel);

            mockMapper.Verify(mapper => mapper.Map(
                It.Is<Requests.GetApprenticeshipsRequest>(request =>
                    request.ProviderId == providerId &&
                    request.PageNumber == filterModel.PageNumber &&
                    request.PageItemCount == ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage &&
                    request.SortField == filterModel.SortField &&
                    request.ReverseSort == filterModel.ReverseSort &&
                    //request.SearchTerm == filterModel.SearchTerm &&
                    request.SelectedEmployer == filterModel.SelectedEmployer &&
                    request.SelectedCourse == filterModel.SelectedCourse &&
                    request.SelectedStatus == filterModel.SelectedStatus &&
                    request.SelectedStartDate == filterModel.SelectedStartDate &&
                    request.SelectedEndDate == filterModel.SelectedEndDate
                    )));
        }

        [Test, MoqAutoData]
        public async Task ThenTheMappedViewModelIsReturned(
            long providerId,
            string sortField,
            ManageApprenticesFilterModel filterModel,
            ManageApprenticesViewModel expectedViewModel,
            [Frozen] Mock<IMapper<Requests.GetApprenticeshipsRequest, ManageApprenticesViewModel>> apprenticeshipMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            apprenticeshipMapper
                .Setup(x => x.Map(It.IsAny<Requests.GetApprenticeshipsRequest>()))
                .ReturnsAsync(expectedViewModel);

            //Act
            var result = await controller.Index(providerId, filterModel) as ViewResult;
            var actualModel = result.Model as ManageApprenticesViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }

        //TODO to move
        //[Test]
        //[MoqInlineAutoData(true, "das-table__sort das-table__sort--desc", "sort-desc")]
        //[MoqInlineAutoData(false, "das-table__sort das-table__sort--asc", "sort-asc")]
        //public async Task ThenTheSortByHeaderClassNameIsSetCorrectly(
        //    bool isReverse,
        //    string expected,
        //    string sortField,
        //    long providerId,
        //    Ma
        //    ManageApprenticesViewModel model,
        //    [Frozen] Mock<IMapper<GetApprenticeshipsRequest, ManageApprenticesViewModel>> mapper,
        //    ManageApprenticesController controller
        //)
        //{
        //    //Arrange
        //    mapper.Setup(x => x.Map(It.Is<GetApprenticeshipsRequest>(c=>c.ProviderId.Equals(providerId)))).ReturnsAsync(model);

        //    //Act
        //    var actual = await controller.Index(providerId, filterModel) as ViewResult;

        //    //Assert
        //    Assert.IsNotNull(actual);
        //    var actualViewModel = actual.Model as ManageApprenticesViewModel;
        //    Assert.IsNotNull(actualViewModel);
        //    Assert.AreEqual(expected, actualViewModel.SortedByHeaderClassName);
        //}
    }
}