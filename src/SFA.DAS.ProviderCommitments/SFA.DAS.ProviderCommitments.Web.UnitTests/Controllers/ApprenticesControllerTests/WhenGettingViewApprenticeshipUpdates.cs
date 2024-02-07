using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingViewApprenticeshipUpdates
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetViewApprenticeshipUpdatesFixture();

            await fixture.Act();

            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetViewApprenticeshipUpdatesFixture();

            var result = await fixture.Act();

            result.VerifyReturnsViewModel().WithModel<ViewApprenticeshipUpdatesViewModel>();
        }
    }

    public class GetViewApprenticeshipUpdatesFixture
    {
        private readonly ApprenticeController _sut;

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ViewApprenticeshipUpdatesRequest _request;

        public GetViewApprenticeshipUpdatesFixture()
        {
            var fixture = new Fixture();
            const long providerId = 123;
            _request = new ViewApprenticeshipUpdatesRequest { ProviderId = providerId, ApprenticeshipHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<ViewApprenticeshipUpdatesViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<ViewApprenticeshipUpdatesViewModel>(_request))
                .ReturnsAsync(viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>(), Mock.Of<ICacheStorageService>(), Mock.Of<ILogger<ApprenticeController>>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ViewApprenticeshipUpdatesViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.ViewApprenticeshipUpdates(_request);
    }
}
