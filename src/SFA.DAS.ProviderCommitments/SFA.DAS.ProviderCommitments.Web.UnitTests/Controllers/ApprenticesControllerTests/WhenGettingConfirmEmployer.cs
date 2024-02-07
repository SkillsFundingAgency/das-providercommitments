using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingConfirmEmployer
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetConfirmEmployerFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetConfirmEmployerFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ConfirmEmployerViewModel>();
        }
    }

    public class GetConfirmEmployerFixture
    {
        private readonly ApprenticeController _sut;
        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ConfirmEmployerRequest _request;
        private readonly long _providerId;

        public GetConfirmEmployerFixture()
        {
            var fixture = new Fixture();
            _request = new ConfirmEmployerRequest { ProviderId = _providerId, EmployerAccountLegalEntityPublicHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<ConfirmEmployerViewModel>();
            _providerId = 123;

            _modelMapperMock
                .Setup(x => x.Map<ConfirmEmployerViewModel>(_request))
                .ReturnsAsync(viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>(), Mock.Of<ILogger<ApprenticeController>>());
        }

        public GetConfirmEmployerFixture WithModelStateErrors()
        {
            _sut.ControllerContext.ModelState.AddModelError("TestError", "Test Error");
            return this;
        }
        
        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmEmployerViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.ConfirmEmployer(_request);
    }
}
