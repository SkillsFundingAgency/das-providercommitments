using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class RecognisePriorLearningRequestToViewModelMapperTests
    {
        private Mock<ICommitmentsApiClient> _commitmentsApiClientMock;
        private Mock<IOuterApiService> _outerApiServiceMock;
        private RecognisePriorLearningRequestToViewModelMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            _outerApiServiceMock = new Mock<IOuterApiService>();
            _mapper = new RecognisePriorLearningRequestToViewModelMapper(_commitmentsApiClientMock.Object, _outerApiServiceMock.Object);
        }

        [Test]
        public async Task Map_SetsIsRplRequired_True_WhenOuterApiReturnsRequired()
        {
            var request = new RecognisePriorLearningRequest { CohortId = 1, DraftApprenticeshipId = 2 };
            var apprenticeship = new GetDraftApprenticeshipResponse { CourseCode = "123", RecognisePriorLearning = true };
            _commitmentsApiClientMock
                .Setup(x => x.GetDraftApprenticeship(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeship);
            _outerApiServiceMock.Setup(x => x.GetRplRequirements(1,2,3,"123"))
                .ReturnsAsync(new GetRplRequirementsResponse { IsRequired = true });

            var result = await _mapper.Map(request);

            result.IsRplRequired.Should().BeTrue();
        }

        [Test]
        public async Task Map_SetsIsRplRequired_False_WhenOuterApiReturnsNotRequired()
        {
            var request = new RecognisePriorLearningRequest { ProviderId = 111, CohortId = 1, DraftApprenticeshipId = 2 };
            var apprenticeship = new GetDraftApprenticeshipResponse { CourseCode = "123", RecognisePriorLearning = true };
            _commitmentsApiClientMock
                .Setup(x => x.GetDraftApprenticeship(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeship);
            _outerApiServiceMock.Setup(x => x.GetRplRequirements(111,1,2,"123")).ReturnsAsync(new GetRplRequirementsResponse { IsRequired = false });

            var result = await _mapper.Map(request);

            result.IsRplRequired.Should().BeFalse();
        }

        [Test]
        public async Task Map_SetsIsRplRequired_True_WhenCourseCodeIsNullOrEmpty()
        {
            var request = new RecognisePriorLearningRequest { CohortId = 1, DraftApprenticeshipId = 2 };
            var apprenticeship = new GetDraftApprenticeshipResponse { CourseCode = null, RecognisePriorLearning = true };
            _commitmentsApiClientMock
                .Setup(x => x.GetDraftApprenticeship(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apprenticeship);

            var result = await _mapper.Map(request);

            result.IsRplRequired.Should().BeTrue();
        }
    }
} 