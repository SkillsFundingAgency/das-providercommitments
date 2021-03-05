using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingSelectEmployer
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
            Assert.AreEqual(typeof(SelectEmployerViewModel), result.Model.GetType());
        }
    }

    public class SelectEmployerFixture
    {
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly SelectEmployerViewModel _viewModel;
        private readonly SelectEmployerRequest _request;

        public SelectEmployerFixture()
        {
            _request = new SelectEmployerRequest { ProviderId = 1, ApprenticeshipId = 1 };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = new List<AccountProviderLegalEntityViewModel>(),
            };

            _modelMapperMock
                .Setup(x => x.Map<SelectEmployerViewModel>(_request))
                .ReturnsAsync(_viewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public SelectEmployerFixture WithModelStateErrors()
        {
            Sut.ControllerContext.ModelState.AddModelError("TestError", "Test Error");
            return this;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<SelectEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.SelectEmployer(_request);
    }
}
