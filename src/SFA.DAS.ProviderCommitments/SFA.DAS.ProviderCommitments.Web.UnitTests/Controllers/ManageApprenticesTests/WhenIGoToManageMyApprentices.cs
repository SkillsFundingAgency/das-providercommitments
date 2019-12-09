using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
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
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller,
            int providerId)
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
            [Frozen]Mock<ICommitmentsService> commitmentsService,
            ManageApprenticesController controller,
            List<ApprenticeshipDetails> approvedApprenticeships)
        {
            //Arrange
            commitmentsService.Setup(x => x.GetApprovedApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.Apprenticeships.Count(), approvedApprenticeships.Count);
        }

/*        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(10, true)]*/
        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(10, true)]
        public void ThenAnyApprenticeshipsIsSet(
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

            commitmentsService.Setup(x => x.GetApprovedApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.AnyApprenticeships, expected);
        }
    }
}