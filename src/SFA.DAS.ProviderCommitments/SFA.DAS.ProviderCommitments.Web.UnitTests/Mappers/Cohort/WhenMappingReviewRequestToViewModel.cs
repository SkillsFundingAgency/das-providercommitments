using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingReviewRequestToViewModel
    {
        [Test]
        public async Task OnlyTheCohortsReadyForReviewAreMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_OnlyTheCohorts_ReadyForReviewForProvider_Are_Mapped();
        }

        [Test]
        public async Task Then_TheCohortReferenceIsMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public async Task Then_EmployerNameIsMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_EmployerName_Is_Mapped();
        }

        [Test]
        public async Task Then_NumberOfApprenticesAreMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public async Task Then_LastMessage_IsMapped_Correctly()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_LastMessage_Is_MappedCorrectly();
        }

        [Test]
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_DateCreatedDescending();
        }

        [Test]
        public void Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }
    }

    public class WhenMappingReviewRequestToViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public CohortsByProviderRequest ReviewRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public ReviewRequestViewModelMapper Mapper { get; set; }
        public ReviewViewModel ReviewViewModel { get; set; }

        public long ProviderId => 1;

        public WhenMappingReviewRequestToViewModelFixture()
        {
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            ReviewRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            Mapper = new ReviewRequestViewModelMapper(CommitmentsApiClient.Object, EncodingService.Object);
        }

        public async Task<WhenMappingReviewRequestToViewModelFixture> Map()
        {
            ReviewViewModel = await Mapper.Map(ReviewRequest);
            return this;
        }

        public void Verify_OnlyTheCohorts_ReadyForReviewForProvider_Are_Mapped()
        {
            Assert.AreEqual(2, ReviewViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(1));
            Assert.IsNotNull(GetCohortInReviewViewModel(2));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

            Assert.AreEqual("1_Encoded", GetCohortInReviewViewModel(1).CohortReference);
            Assert.AreEqual("2_Encoded", GetCohortInReviewViewModel(2).CohortReference);
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.AreEqual("Employer1", GetCohortInReviewViewModel(1).EmployerName);
            Assert.AreEqual("Employer2", GetCohortInReviewViewModel(2).EmployerName);
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(100, GetCohortInReviewViewModel(1).NumberOfApprentices);
            Assert.AreEqual(200, GetCohortInReviewViewModel(2).NumberOfApprentices);
        }

        public void Verify_LastMessage_Is_MappedCorrectly()
        {
            Assert.AreEqual("No message added", GetCohortInReviewViewModel(1).LastMessage);
            Assert.AreEqual("This is latestMessage from employer", GetCohortInReviewViewModel(2).LastMessage);
        }

        public void Verify_Ordered_By_DateCreatedDescending()
        {
            Assert.AreEqual("Employer1", ReviewViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer2", ReviewViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, ReviewViewModel.ProviderId);
        }

        private GetCohortsResponse CreateGetCohortsResponse()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new CohortSummary
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now.AddMinutes(10)
                },
                new CohortSummary
                {
                    CohortId = 2,
                    AccountId = 2,
                    ProviderId = 1,
                    LegalEntityName = "Employer2",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now.AddMinutes(5),
                    LatestMessageFromEmployer = new Message("This is latestMessage from employer", DateTime.Now.AddMinutes(-2))
                },
                new CohortSummary
                {
                    CohortId = 3,
                    AccountId = 3,
                    ProviderId = 1,
                    LegalEntityName = "Employer3",
                    NumberOfDraftApprentices = 300,
                    IsDraft = true,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddMinutes(1)
                },
                 new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 4,
                    ProviderId = 1,
                    LegalEntityName = "Employer4",
                    NumberOfDraftApprentices = 400,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }

        private long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private ReviewCohortSummaryViewModel GetCohortInReviewViewModel(long id)
        {
            return ReviewViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
