using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Web.Mappers.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.OverlappingTrainingDateRequest
{
    [TestFixture]
    public class DraftApprenticeshipOverlapOptionRequestToViewModelMapperTests
    {
        private DraftApprenticeshipOverlapOptionRequestToViewModelMapper _mapper;
        private DraftApprenticeshipOverlapOptionRequest _source;
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private Mock<IFeatureTogglesService<ProviderFeatureToggle>> _featureToggleService;
        private GetApprenticeshipResponse _apiResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _source = fixture.Build<DraftApprenticeshipOverlapOptionRequest>()
                .Create();

            _apiResponse = fixture.Create<GetApprenticeshipResponse>();
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.GetApprenticeship(_source.ApprenticeshipId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apiResponse);

            _featureToggleService = new Mock<IFeatureTogglesService<ProviderFeatureToggle>>();
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix))
                .Returns(new ProviderFeatureToggle { IsEnabled = true });

            _mapper = new DraftApprenticeshipOverlapOptionRequestToViewModelMapper(_featureToggleService.Object, _commitmentsApiClient.Object);
        }

        [Test]
        public async Task ThenDraftApprenticeshipHashedIdIsMappedCorrectly()
        {
            var result = await _mapper.Map(_source);
            Assert.AreEqual(_source.DraftApprenticeshipHashedId, result.DraftApprenticeshipHashedId);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenOverlappingTrainingDateRequestToggleEnabledIsMappedCorrectly(bool isEnabled)
        {
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix))
                .Returns(new ProviderFeatureToggle { IsEnabled = isEnabled });
            var result = await _mapper.Map(_source);
            Assert.AreEqual(isEnabled, result.OverlappingTrainingDateRequestToggleEnabled);
        }

        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Completed)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Live)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Stopped)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Paused)]
        public async Task ThenStatusIsMappedCorrectly(CommitmentsV2.Types.ApprenticeshipStatus apprenticeshipStatus)
        {
            _apiResponse.Status = apprenticeshipStatus;
            var result = await _mapper.Map(_source);
            Assert.AreEqual(apprenticeshipStatus, result.Status);
        }

        [Test]
        public async Task ThenEnableStopRequestEmail_MappedCorrectly_WhenFeatureIsFalse()
        {
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix))
                .Returns(new ProviderFeatureToggle { IsEnabled = false });
            _apiResponse.Status = CommitmentsV2.Types.ApprenticeshipStatus.Live;
            var result = await _mapper.Map(_source);
            Assert.AreEqual(false, result.EnableStopRequestEmail);
        }

        [Test]
        public async Task ThenEnableStopRequestEmail_MappedCorrectly_WhenFeatureIsTrue()
        {
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix))
                .Returns(new ProviderFeatureToggle { IsEnabled = true });
            _apiResponse.Status = CommitmentsV2.Types.ApprenticeshipStatus.Live;
            var result = await _mapper.Map(_source);
            Assert.AreEqual(true, result.EnableStopRequestEmail);
        }

        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Completed, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Live, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Stopped, true)]
        [TestCase(CommitmentsV2.Types.ApprenticeshipStatus.Paused, true)]
        public async Task ThenEnableStopRequestEmailIsMappedCorrectly(CommitmentsV2.Types.ApprenticeshipStatus apprenticeshipStatus, bool sendEmail)
        {
            _featureToggleService.Setup(x => x.GetFeatureToggle(ProviderFeature.OverlappingTrainingDateWithoutPrefix))
                .Returns(new ProviderFeatureToggle { IsEnabled = true });
            _apiResponse.Status = apprenticeshipStatus;
            var result = await _mapper.Map(_source);
            Assert.AreEqual(sendEmail, result.EnableStopRequestEmail);
        }
    }
}
