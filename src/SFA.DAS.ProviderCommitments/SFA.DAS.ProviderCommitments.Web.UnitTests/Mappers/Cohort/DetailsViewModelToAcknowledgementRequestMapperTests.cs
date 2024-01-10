using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class DetailsViewModelToAcknowledgementRequestMapperTests
    {
        private DetailsViewModelToAcknowledgementRequestMapper _mapper;
        private DetailsViewModel _source;
        private Mock<IOuterApiClient> _apiClient;
        private PostCohortDetailsRequest _apiRequest;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _source = fixture.Create<DetailsViewModel>();

            _apiClient = new Mock<IOuterApiClient>();

            _apiClient.Setup(x => x.Post<PostCohortDetailsResponse>(It.IsAny<PostCohortDetailsRequest>()))
                .Callback((IPostApiRequest request) => _apiRequest = (PostCohortDetailsRequest) request)
                .ReturnsAsync(() => new PostCohortDetailsResponse());

            _mapper = new DetailsViewModelToAcknowledgementRequestMapper(_apiClient.Object,
                Mock.Of<IAuthenticationService>());
        }

        [Test]
        public async Task Cohort_Approval_Is_Submitted_Correctly()
        {
            _source.Selection = CohortDetailsOptions.Approve;

            await _mapper.Map(_source);

            Assert.That(_apiRequest.ProviderId, Is.EqualTo(_source.ProviderId));
            Assert.That(_apiRequest.CohortId, Is.EqualTo(_source.CohortId));
            Assert.That(((PostCohortDetailsRequest.Body)_apiRequest.Data).SubmissionType, Is.EqualTo(PostCohortDetailsRequest.CohortSubmissionType.Approve));
            Assert.That(((PostCohortDetailsRequest.Body)_apiRequest.Data).Message, Is.EqualTo(_source.ApproveMessage));
        }

        [Test]
        public async Task Cohort_Send_Is_Submitted_Correctly()
        {
            _source.Selection = CohortDetailsOptions.Send;

            await _mapper.Map(_source);

            Assert.That(_apiRequest.ProviderId, Is.EqualTo(_source.ProviderId));
            Assert.That(_apiRequest.CohortId, Is.EqualTo(_source.CohortId));
            Assert.That(((PostCohortDetailsRequest.Body)_apiRequest.Data).SubmissionType, Is.EqualTo(PostCohortDetailsRequest.CohortSubmissionType.Send));
            Assert.That(((PostCohortDetailsRequest.Body)_apiRequest.Data).Message, Is.EqualTo(_source.SendMessage));
        }

        [Test]
        public async Task Cohort_Send_SaveStatus_Is_Determined_Correctly()
        {
            var request = new DetailsViewModel
            {
                Selection = CohortDetailsOptions.Send
            };

            var result = await _mapper.Map(request);

            Assert.That(result.SaveStatus, Is.EqualTo(SaveStatus.AmendAndSend));
        }

        [TestCase(true,SaveStatus.Approve)]
        [TestCase(false, SaveStatus.ApproveAndSend)]
        public async Task Cohort_Approval_SaveStatus_Is_Determined_Correctly(bool isApprovedByEmployer, SaveStatus expectedStatus)
        {
            var request = new DetailsViewModel
            {
                Selection = CohortDetailsOptions.Approve,
                IsApprovedByEmployer = isApprovedByEmployer
            };

            var result = await _mapper.Map(request);

            Assert.That(result.SaveStatus, Is.EqualTo(expectedStatus));
        }
    }
}
