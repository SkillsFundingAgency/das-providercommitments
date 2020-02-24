using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests.Apprentice;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingSelectNewEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new SelectEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new SelectEmployerFixture();

            var result = await fixture.Act() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(Web.Models.Apprentice.SelectNewEmployerViewModel), result.Model.GetType());
        }
    }

    public class SelectEmployerFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Web.Models.Apprentice.SelectNewEmployerViewModel _viewModel;
        private readonly SelectNewEmployerRequest _request;

        public SelectEmployerFixture()
        {
            _request = new SelectNewEmployerRequest { ProviderId = 1, ApprenticeshipId = 1 };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = new Web.Models.Apprentice.SelectNewEmployerViewModel
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityViewModel>(),
            };

            _modelMapperMock
                .Setup(x => x.Map<Web.Models.Apprentice.SelectNewEmployerViewModel>(_request))
                .ReturnsAsync(_viewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object);
        }

        public SelectEmployerFixture WithModelStateErrors()
        {
            Sut.ControllerContext.ModelState.AddModelError("TestError", "Test Error");
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<Web.Models.Apprentice.SelectNewEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.SelectEmployer(_request);
    }
}
