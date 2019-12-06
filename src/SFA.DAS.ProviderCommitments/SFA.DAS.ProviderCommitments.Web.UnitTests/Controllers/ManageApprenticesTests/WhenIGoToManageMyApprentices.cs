using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
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

            var view = ((ViewResult) result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.ProviderId, actualProviderId);
        }

        [Test]
        public void ThenTheApprovedApprenticesAreSet()
        {
            //Arrange
            var expectedApprenticeshipsCount = 1;
            var approvedApprenticeships = new List<ApprenticeshipDetails>
            {
                new ApprenticeshipDetails
                {
                    Alerts = "alert",
                    ApprenticeName = "apprentice name",
                    CourseName = "course name",
                    EmployerName = "employer name",
                    PlannedStartDate = DateTime.UtcNow,
                    PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                    Status = "status",
                    Uln = "uln"
                }
            };

            _commitmentsService.Setup(x => x.GetApprovedApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = _controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.Apprenticeships.Count(), expectedApprenticeshipsCount);
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(10, true)]
        public void ThenAnyApprenticeshipsIsSet(int numberOfApprenticeships, bool expected)
        {
            //Arrange
            var approvedApprenticeship = new ApprenticeshipDetails
            {
                Alerts = "alert",
                ApprenticeName = "apprentice name",
                CourseName = "course name",
                EmployerName = "employer name",
                PlannedStartDate = DateTime.UtcNow,
                PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                Status = "status",
                Uln = "uln"
            };

            var approvedApprenticeships = new List<ApprenticeshipDetails>();

            for (int i = 0; i < numberOfApprenticeships; i++)
            {
                approvedApprenticeships.Add(approvedApprenticeship);
            }

            _commitmentsService.Setup(x => x.GetApprovedApprenticeships(It.IsAny<uint>()))
                .ReturnsAsync(approvedApprenticeships);

            //Act
            var result = _controller.Index(1);
            var view = ((ViewResult)result.Result).Model as ManageApprenticesViewModel;

            //Assert
            Assert.AreEqual(view.AnyApprenticeships, expected);
        }
    }
}