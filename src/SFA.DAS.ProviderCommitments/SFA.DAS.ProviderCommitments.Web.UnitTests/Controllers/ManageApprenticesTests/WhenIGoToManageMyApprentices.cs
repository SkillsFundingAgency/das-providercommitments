using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ManageApprenticesTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        private ManageApprenticesController _controller;

        [SetUp]
        public void Arrange()
        {
            _controller = new ManageApprenticesController();
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