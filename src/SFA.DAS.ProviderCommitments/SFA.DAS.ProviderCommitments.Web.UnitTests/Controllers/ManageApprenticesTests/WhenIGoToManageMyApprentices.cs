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
    }
}
