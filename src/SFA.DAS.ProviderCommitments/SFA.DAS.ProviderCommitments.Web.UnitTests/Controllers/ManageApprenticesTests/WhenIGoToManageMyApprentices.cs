using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
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
            [Frozen]Mock<ICommitmentsService> commitmentsService,
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
            uint providerId,
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
            GetApprenticeshipsFilteredResult apprenticeshipsResult,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>(),It.IsAny<uint>()))
                .ReturnsAsync(apprenticeshipsResult);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            view.Apprenticeships.Should().BeEquivalentTo(apprenticeshipsResult.Apprenticeships);
        }

        [Test, MoqAutoData]
        public void ThenTheTotalNumberOfApprenticesIsSet(
            GetApprenticeshipsFilteredResult apprenticeshipsResult,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync(apprenticeshipsResult);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            view.FilterModel.NumberOfRecordsFound.Should().Be((int)apprenticeshipsResult.NumberOfRecordsFound);
        }

        
        [Test, MoqAutoData]
        public void ThenThePageNumberIsSet(
            GetApprenticeshipsFilteredResult apprenticeshipsResult,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            uint expectedPageNumber = 4;
            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>(), expectedPageNumber))
                .ReturnsAsync(apprenticeshipsResult);

            //Act
            var result = controller.Index(1, expectedPageNumber);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            view.FilterModel.NumberOfRecordsFound.Should().Be((int)apprenticeshipsResult.NumberOfRecordsFound);
        }

        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(2, true)]
        public void ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNotNull(
            int numberOfApprenticeships, 
            bool expected,
            ApprenticeshipDetails approvedApprenticeship,
            GetApprenticeshipsFilteredResult apprenticeshipsResult,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
           var apprenticeships = new List<ApprenticeshipDetails>();

            for (int i = 0; i < numberOfApprenticeships; i++)
            {
                apprenticeships.Add(approvedApprenticeship);
            }

            apprenticeshipsResult.Apprenticeships = apprenticeships;

            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync(apprenticeshipsResult);

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

            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>(), It.IsAny<uint>()))
                .ReturnsAsync((GetApprenticeshipsFilteredResult)null);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.AnyApprenticeships, expected);
        }
    }
}
