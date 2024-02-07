using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenGettingTrainingDates
    {
        private GetTrainingDatesFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new GetTrainingDatesFixture();
        }

        [Test]
        public async Task ThenCallsModelMapper()
        {
            await _fixture.Act();

            _fixture.Verify_ModelMapperWasCalled(Times.Once());
        }

        [Test]
        public async Task ThenReturnsView()
        {
            var result = await _fixture.Act();

            result.VerifyReturnsViewModel().WithModel<TrainingDatesViewModel>();
        }

        private class GetTrainingDatesFixture
        {
            private readonly Mock<Interfaces.ICookieStorageService<IndexRequest>> _cookieStorageServiceMock;
            private readonly Mock<IModelMapper> _modelMapperMock;
            private readonly TrainingDatesRequest _request;
            private readonly ApprenticeController _sut;
            private readonly TrainingDatesViewModel _viewModel;

            public GetTrainingDatesFixture()
            {
                _request = new TrainingDatesRequest
                {
                    ProviderId = 2342,
                    ApprenticeshipHashedId = "KG34DF989"
                };
                _viewModel = new TrainingDatesViewModel
                {
                    ProviderId = 2342,
                    CacheKey = Guid.NewGuid()
                };
                _cookieStorageServiceMock = new Mock<Interfaces.ICookieStorageService<IndexRequest>>();
                _modelMapperMock = new Mock<IModelMapper>();
                _modelMapperMock
                    .Setup(x => x.Map<TrainingDatesViewModel>(_request))
                    .ReturnsAsync(_viewModel);

                _sut = new ApprenticeController(_modelMapperMock.Object, _cookieStorageServiceMock.Object,
                    Mock.Of<ICommitmentsApiClient>(), 
                    Mock.Of<IOuterApiService>(), 
                    Mock.Of<ICacheStorageService>(), 
                    Mock.Of<ILogger<ApprenticeController>>());
            }

            public Task<IActionResult> Act() => _sut.TrainingDates(_request);

            public void Verify_ModelMapperWasCalled(Times times) =>
                _modelMapperMock.Verify(x => x.Map<TrainingDatesViewModel>(_request), times);
        }
    }
}