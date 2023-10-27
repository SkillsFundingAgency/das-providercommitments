using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenGettingChangeOption
    {
        private GetChangeOptionFixture _fixture;
        
        [SetUp]
        public void Arrange()
        {
            _fixture = new GetChangeOptionFixture();
        }

        [Test]
        public async Task ThenVerifyMapperWasCalled()
        {
            await _fixture.ChangeOption();

            _fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsViewModel()
        {
            var result = await _fixture.ChangeOption();

            _fixture.VerifyViewModel(result as ViewResult);
        }
    }

    public class GetChangeOptionFixture
    {
        public ApprenticeController Controller { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ChangeOptionRequest _request;
        private readonly ChangeOptionViewModel _viewModel;

        public GetChangeOptionFixture()
        {
            var fixture = new Fixture();

            _request = fixture.Create<ChangeOptionRequest>();
            _viewModel = fixture.Create<ChangeOptionViewModel>();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(m => m.Map<ChangeOptionViewModel>(_request)).ReturnsAsync(_viewModel);

            Controller = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public async Task<IActionResult> ChangeOption()
        {
            var result = await Controller.ChangeOption(_request);

            return result as ViewResult;
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(m => m.Map<ChangeOptionViewModel>(_request));
        }

        public void VerifyViewModel(ViewResult viewResult)
        {
            var viewModel = viewResult.Model as ChangeOptionViewModel;

            Assert.AreEqual(_viewModel, viewModel);
        }
    }
}
