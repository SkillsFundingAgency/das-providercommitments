using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        private ManageApprenticesController _controller;
        private Mock<ICommitmentsService> _commitmentsService;

        [SetUp]
        public void Arrange()
        {
            _commitmentsService = new Mock<ICommitmentsService>();
            _controller = new ManageApprenticesController(_commitmentsService.Object);
        }

        [Test]
        public void IfCalledWithAnInvalidRequestShouldGetBadResponseReturned()
        {
            //Arrange
            _controller.ModelState.AddModelError("test", "test");
            //Act
            var result = _controller.Index(1);
            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void ThenTheProviderIdIsPassedToTheViewModel()
        {
            //Arrange
            var actualProviderId = 1;
            //Act
            var result = _controller.Index(actualProviderId);
            var view = ((ViewResult) result).Model as ManageApprenticesViewModel;
            //Assert
            Assert.AreEqual(view.ProviderId, actualProviderId);
        }
    }
}