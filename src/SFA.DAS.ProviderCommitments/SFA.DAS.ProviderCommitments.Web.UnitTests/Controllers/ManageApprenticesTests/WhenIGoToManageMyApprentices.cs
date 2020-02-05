﻿using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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
            ManageApprenticesFilterModel filterModel,
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
        public async Task ThenTheMappedViewModelIsReturned(
            long providerId,
            ManageApprenticesFilterModel filterModel,
            ManageApprenticesViewModel expectedViewModel,
            [Frozen] Mock<IModelMapper> apprenticeshipMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            apprenticeshipMapper
                .Setup(mapper => mapper.Map<ManageApprenticesViewModel>(
                    It.Is<Requests.GetApprenticeshipsRequest>(request =>
                            request.ProviderId == providerId &&
                            request.PageNumber == filterModel.PageNumber &&
                            request.PageItemCount == ProviderCommitmentsWebConstants.NumberOfApprenticesPerSearchPage &&
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
            var actualModel = result.Model as ManageApprenticesViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }
    }
}