using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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
        public ApprenticeController Sut { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly ViewApprenticeshipUpdatesViewModel _viewModel;
        private readonly ViewApprenticeshipUpdatesRequest _request;
        private readonly long _providerId;

        public GetViewApprenticeshipUpdatesFixture()
        {
            var fixture = new Fixture();
            _providerId = 123;
            _request = new ViewApprenticeshipUpdatesRequest { ProviderId = _providerId, ApprenticeshipHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            _viewModel = fixture.Create<ViewApprenticeshipUpdatesViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<ViewApprenticeshipUpdatesViewModel>(_request))
                .ReturnsAsync(_viewModel);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<SFA.DAS.ProviderCommitments.Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ViewApprenticeshipUpdatesViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await Sut.ViewApprenticeshipUpdates(_request);
    }
}
