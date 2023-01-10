using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Azure.Documents;
using Moq;
using NUnit.Framework;
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

            Assert.AreEqual(_source.ProviderId, _apiRequest.ProviderId);
            Assert.AreEqual(_source.CohortId, _apiRequest.CohortId);
            Assert.AreEqual(PostCohortDetailsRequest.CohortSubmissionType.Approve, ((PostCohortDetailsRequest.Body)_apiRequest.Data).SubmissionType);
            Assert.AreEqual(_source.ApproveMessage, ((PostCohortDetailsRequest.Body)_apiRequest.Data).Message);
        }

        [Test]
        public async Task Cohort_Send_Is_Submitted_Correctly()
        {
            _source.Selection = CohortDetailsOptions.Send;

            await _mapper.Map(_source);

            Assert.AreEqual(_source.ProviderId, _apiRequest.ProviderId);
            Assert.AreEqual(_source.CohortId, _apiRequest.CohortId);
            Assert.AreEqual(PostCohortDetailsRequest.CohortSubmissionType.Send, ((PostCohortDetailsRequest.Body)_apiRequest.Data).SubmissionType);
            Assert.AreEqual(_source.SendMessage, ((PostCohortDetailsRequest.Body)_apiRequest.Data).Message);
        }

        [Test]
        public async Task Cohort_Send_SaveStatus_Is_Determined_Correctly()
        {
            var request = new DetailsViewModel
            {
                Selection = CohortDetailsOptions.Send
            };

            var result = await _mapper.Map(request);

            Assert.AreEqual(SaveStatus.AmendAndSend, result.SaveStatus);
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

            Assert.AreEqual(expectedStatus, result.SaveStatus);
        }
    }
}
