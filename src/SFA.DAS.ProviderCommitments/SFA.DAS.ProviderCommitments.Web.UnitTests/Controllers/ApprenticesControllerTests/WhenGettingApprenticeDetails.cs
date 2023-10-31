using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingApprenticeDetails
    {
        [Test]
        public async Task ThenCallsModelMapper()
        {
            var fixture = new GetApprenticeDetailsFixture();
            await fixture.Act();
            fixture.VerifyMapperWasCalled();
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var fixture = new GetApprenticeDetailsFixture();
            var result = await fixture.Act();
            result.VerifyReturnsViewModel().WithModel<DetailsViewModel>();
        }
    }

    public class GetApprenticeDetailsFixture
    {
        private readonly ApprenticeController _sut;

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly DetailsRequest _request;

        public GetApprenticeDetailsFixture()
        {
            var fixture = new Fixture();
            const long providerId = 123;
            _request = new DetailsRequest { ProviderId = providerId, ApprenticeshipHashedId = "XYZ" };
            _modelMapperMock = new Mock<IModelMapper>();
            var viewModel = fixture.Create<DetailsViewModel>();

            _modelMapperMock
                .Setup(x => x.Map<DetailsViewModel>(_request))
                .ReturnsAsync(viewModel);

            _sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(), Mock.Of<ICommitmentsApiClient>());
        }

        public void VerifyMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<DetailsViewModel>(_request));
        }

        public async Task<IActionResult> Act() => await _sut.Details(_request);
    }
}
