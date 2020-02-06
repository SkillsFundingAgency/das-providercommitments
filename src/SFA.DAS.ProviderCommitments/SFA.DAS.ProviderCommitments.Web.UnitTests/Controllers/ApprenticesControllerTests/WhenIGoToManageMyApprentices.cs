using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        [Test, MoqAutoData]
        public void IfCalledWithAnInvalidRequestShouldGetBadResponseReturned(
            long providerId,
            ApprenticesFilterModel filterModel,
            ApprenticeController controller)
        {
            //Arrange
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = controller.Index(providerId, filterModel);

            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test, MoqAutoData]
        public async Task ThenTheMappedViewModelIsReturned(
            long providerId,
            ApprenticesFilterModel filterModel,
            IndexViewModel expectedViewModel,
            [Frozen] Mock<IModelMapper> apprenticeshipMapper,
            ApprenticeController controller)
        {
            //Arrange
            apprenticeshipMapper
                .Setup(mapper => mapper.Map<IndexViewModel>(
                    It.Is<Requests.GetApprenticeshipsRequest>(request =>
                            request.ProviderId == providerId &&
                            request.PageNumber == filterModel.PageNumber &&
                            request.PageItemCount == Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage &&
                            request.SortField == filterModel.SortField &&
                            request.ReverseSort == filterModel.ReverseSort &&
                            request.SearchTerm == filterModel.SearchTerm &&
                            request.SelectedEmployer == filterModel.SelectedEmployer &&
                            request.SelectedCourse == filterModel.SelectedCourse &&
                            request.SelectedStatus == filterModel.SelectedStatus &&
                            request.SelectedStartDate == filterModel.SelectedStartDate &&
                            request.SelectedEndDate == filterModel.SelectedEndDate)))
                .ReturnsAsync(expectedViewModel);

            //Act
            var result = await controller.Index(providerId, filterModel) as ViewResult;
            var actualModel = result.Model as IndexViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }
    }
}