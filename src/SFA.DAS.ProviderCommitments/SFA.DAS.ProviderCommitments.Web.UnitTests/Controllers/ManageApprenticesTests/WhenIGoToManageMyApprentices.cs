using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        [Test, MoqAutoData]
        public void IfCalledWithAnInvalidRequestShouldGetBadResponseReturned(
            [Frozen]Mock<IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>> apprenticeshipMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = controller.Index(1);

            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test, MoqAutoData]
        public void ThenTheProviderIdIsPassedToTheViewModel(
            long providerId,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            
            //Act
            var result = controller.Index(providerId);

            var view = ((ViewResult) result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.ProviderId, providerId);
        }

        [Test, MoqAutoData]
        public void ThenTheApprovedApprenticesAreSet(
            GetApprenticeshipsResponse approvedApprenticeships,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            view.Apprenticeships.Should().BeEquivalentTo(approvedApprenticeships.Apprenticeships);
        }

        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(2, true)]
        public void ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNotNull(
            int numberOfApprenticeships, 
            bool expected,
            ApprenticeshipDetailsResponse approvedApprenticeship,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            var approvedApprenticeships = new List<ApprenticeshipDetailsResponse>();

            for (int i = 0; i < numberOfApprenticeships; i++)
            {
                approvedApprenticeships.Add(approvedApprenticeship);
            }

            var response = new GetApprenticeshipsResponse
            {
                Apprenticeships = approvedApprenticeships,
            };

            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(response);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.AnyApprenticeships, expected);
        }

        [Test, MoqAutoData]
        public void ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNull(
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            var expected = false;

            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((GetApprenticeshipsResponse) null);
          
          //Act
            var result = controller.Index(1);
            var actualModel = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }
      
        [Test, MoqAutoData]
        public void ThenTheMappedViewModelIsReturned(
            ManageApprenticesViewModel expectedViewModel,
            [Frozen]Mock<IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>> apprenticeshipMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            apprenticeshipMapper.Setup(x => x.Map(It.IsAny<GetApprenticeshipsRequest>()))
                .ReturnsAsync(expectedViewModel);

            //Act
            var result = controller.Index(1);
            var actualModel = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }

        [Test]
        [MoqInlineAutoData(true, "das-table__sort das-table__sort--desc")]
        [MoqInlineAutoData(false, "das-table__sort das-table__sort--asc")]
        public void ThenTheSortByHeaderClassNameIsSetCorrectly(
            bool isReverse,
            string expected,
            [Frozen] Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller
        )
        {
            //Arrange
            
            //Act
            var result = controller.Index(1, reverseSort:isReverse);
            var model = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(expected, model.SortedByHeaderClassName);
        }
    }
}
