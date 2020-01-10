using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Testing.Builders;

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
        public void Then_The_ApprenticeshipDetailsViewModels_Are_Set_From_Mapper(
            uint providerId,
            List<ApprenticeshipDetails> approvedApprenticeships,
            ApprenticeshipDetailsViewModel viewModelFromMapper,
            [Frozen] Mock<ICommitmentsService> commitmentsService,
            [Frozen] Mock<IMapper<ApprenticeshipDetails, ApprenticeshipDetailsViewModel>> mockMapper,
            ManageApprenticesController controller)
        {
            //Arrange
            commitmentsService
                .Setup(x => x.GetApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);
            mockMapper
                .Setup(mapper => mapper.Map(It.IsAny<ApprenticeshipDetails>()))
                .ReturnsAsync(viewModelFromMapper);

            //Act
            var result = controller.Index(providerId);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            view.Apprenticeships.Should().AllBeEquivalentTo(viewModelFromMapper);
        }

        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(2, true)]
        public void ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNotNull(
            int numberOfApprenticeships, 
            bool expected,
            ApprenticeshipDetails approvedApprenticeship,
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller)
        {
            //Arrange
            var approvedApprenticeships = new List<ApprenticeshipDetails>();

            for (int i = 0; i < numberOfApprenticeships; i++)
            {
                approvedApprenticeships.Add(approvedApprenticeship);
            }

            commitmentsService.Setup(x => x.GetApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

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
            List<ApprenticeshipDetails> approvedApprenticeships = null;
            commitmentsService
                .Setup(x => x.GetApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.IsFalse(view.AnyApprenticeships);
        }
    }
}
