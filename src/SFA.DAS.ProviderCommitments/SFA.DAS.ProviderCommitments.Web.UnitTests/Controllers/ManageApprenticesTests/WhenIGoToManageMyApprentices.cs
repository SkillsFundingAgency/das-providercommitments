using System.Collections.Generic;
using System.Threading;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest;

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

        [Test]
        [MoqInlineAutoData(true, "das-table__sort das-table__sort--desc")]
        [MoqInlineAutoData(false, "das-table__sort das-table__sort--asc")]
        public void ThenTheSortByHeaderClassNameIsSetCorrectly(
            bool isReverse,
            string expected,
            [Frozen] Mock<IMapper<GetApprenticeshipsRequest, ManageApprenticesViewModel>> mapper,
            ManageApprenticesController controller
        )
        {
            //Arrange
            var model = new ManageApprenticesViewModel();
            model.ReverseSort = isReverse;

            mapper.Setup(x => x.Map(It.IsAny<GetApprenticeshipsRequest>())).ReturnsAsync(model);

            //Act
            controller.Index(1);

            //Assert
            Assert.AreEqual(expected, model.SortedByHeaderClassName);
        }
    }
}
