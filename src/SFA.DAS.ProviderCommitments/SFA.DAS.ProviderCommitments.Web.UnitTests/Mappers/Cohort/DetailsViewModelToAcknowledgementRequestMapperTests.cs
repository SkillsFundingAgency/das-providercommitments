using System.Threading.Tasks;
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
        private Mock<IOuterApiClient> _apiClient;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IOuterApiClient>();

            _mapper = new DetailsViewModelToAcknowledgementRequestMapper(_apiClient.Object,
                Mock.Of<IAuthenticationService>());
        }

        [Test]
        public async Task Cohort_Approval_Is_Submitted_Correctly()
        {
            var request = new DetailsViewModel
            {
                Selection = CohortDetailsOptions.Approve
            };

            await _mapper.Map(request);

            _apiClient.Verify(x => x.Post<PostCohortDetailsResponse>(It.Is<PostCohortDetailsRequest>(r =>
                    ((PostCohortDetailsRequest.Body)r.Data).SubmissionType == PostCohortDetailsRequest.CohortSubmissionType.Approve
                    && ((PostCohortDetailsRequest.Body)r.Data).Message == request.ApproveMessage
                    && r.ProviderId == request.ProviderId
                    && r.CohortId == request.CohortId
                    )),
                Times.Once);
        }

        [Test]
        public async Task Cohort_Send_Is_Submitted_Correctly()
        {
            var request = new DetailsViewModel
            {
                Selection = CohortDetailsOptions.Send
            };

            var result = await _mapper.Map(request);

            _apiClient.Verify(x => x.Post<PostCohortDetailsResponse>(It.Is<PostCohortDetailsRequest>(r =>
                    ((PostCohortDetailsRequest.Body)r.Data).SubmissionType == PostCohortDetailsRequest.CohortSubmissionType.Send
                    && ((PostCohortDetailsRequest.Body)r.Data).Message == request.SendMessage
                    && r.ProviderId == request.ProviderId
                    && r.CohortId == request.CohortId
                )),
                Times.Once);
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
