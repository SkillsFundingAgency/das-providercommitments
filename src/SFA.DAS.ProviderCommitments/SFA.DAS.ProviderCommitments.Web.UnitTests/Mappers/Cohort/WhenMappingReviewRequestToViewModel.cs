using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;

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
        public async Task Then_DateReceived_IsMapped_Correctly()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_DateReceived_Is_Mapped();
        }

        [Test]
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_DateCreatedDescending();
        }

        [Test]
        public async Task Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }

        [TestCase("", false, "5_Encoded", "2_Encoded")]
        [TestCase("Employer", false, "5_Encoded", "2_Encoded")]
        [TestCase("Employer", true, "2_Encoded", "5_Encoded")]
        [TestCase("CohortReference", false, "1_Encoded", "5_Encoded")]
        [TestCase("CohortReference", true, "5_Encoded", "1_Encoded")]
        [TestCase("DateReceived", false, "5_Encoded", "2_Encoded")]
        [TestCase("DateReceived", true, "5_Encoded", "2_Encoded")]
        public async Task Then_Sort_IsApplied_Correctly(string sortField, bool reverse, string expectedFirstId, string expectedLastId)
        {
            var fixture = new WhenMappingReviewRequestToViewModelFixture().WithSortApplied(sortField, reverse);
            await fixture.Map();

            fixture.Verify_Sort_IsApplied(expectedFirstId, expectedLastId);
        }
    }

    public class WhenMappingReviewRequestToViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; }
        public Mock<IPasAccountApiClient> PasAccountApiClient { get; set; }
        public Mock<IUrlHelper> UrlHelper { get; }
        public CohortsByProviderRequest ReviewRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public ReviewRequestViewModelMapper Mapper { get; set; }
        public ReviewViewModel ReviewViewModel { get; set; }

        public long ProviderId => 1;
        public DateTime Now = DateTime.Now;

        public WhenMappingReviewRequestToViewModelFixture()
        {
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            ReviewRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
          
            ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

            PasAccountApiClient = new Mock<IPasAccountApiClient>();
            PasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            UrlHelper = new Mock<IUrlHelper>();
            UrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            Mapper = new ReviewRequestViewModelMapper(CommitmentsApiClient.Object, ProviderRelationshipsApiClient.Object, UrlHelper.Object, PasAccountApiClient.Object, EncodingService.Object);
        }

        public async Task<WhenMappingReviewRequestToViewModelFixture> Map()
        {
            ReviewViewModel = await Mapper.Map(ReviewRequest);
            return this;
        }

        public WhenMappingReviewRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            ReviewRequest.SortField = sortField;
            ReviewRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_OnlyTheCohorts_ReadyForReviewForProvider_Are_Mapped()
        {
            Assert.AreEqual(3, ReviewViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(1));
            Assert.IsNotNull(GetCohortInReviewViewModel(2));
            Assert.IsNotNull(GetCohortInReviewViewModel(5));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(3));

            Assert.AreEqual("1_Encoded", GetCohortInReviewViewModel(1).CohortReference);
            Assert.AreEqual("2_Encoded", GetCohortInReviewViewModel(2).CohortReference);
            Assert.AreEqual("5_Encoded", GetCohortInReviewViewModel(5).CohortReference);
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

        public void Verify_DateReceived_Is_Mapped()
        {
            Assert.AreEqual(Now.AddMinutes(-10), GetCohortInReviewViewModel(1).DateReceived);
            Assert.AreEqual(Now.AddMinutes(-2), GetCohortInReviewViewModel(2).DateReceived);
        }

        public void Verify_Ordered_By_DateCreatedDescending()
        {
            Assert.AreEqual("1_Employer5", ReviewViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer2", ReviewViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, ReviewViewModel.ProviderId);
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.AreEqual(firstId, ReviewViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual(lastId, ReviewViewModel.Cohorts.Last().CohortReference);
        }

        private GetCohortsResponse CreateGetCohortsResponse()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new()
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = Now.AddMinutes(-10)
                },
                new()
                {
                    CohortId = 2,
                    AccountId = 2,
                    ProviderId = 1,
                    LegalEntityName = "Employer2",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = Now.AddMinutes(-5),
                    LatestMessageFromEmployer = new Message("This is latestMessage from employer", Now.AddMinutes(-2))
                },
                new()
                {
                    CohortId = 3,
                    AccountId = 3,
                    ProviderId = 1,
                    LegalEntityName = "Employer3",
                    NumberOfDraftApprentices = 300,
                    IsDraft = true,
                    WithParty = Party.Employer,
                    CreatedOn = Now.AddMinutes(-1)
                },
                new()
                {
                    CohortId = 4,
                    AccountId = 4,
                    ProviderId = 1,
                    LegalEntityName = "Employer4",
                    NumberOfDraftApprentices = 400,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = Now
                },
                new()
                {
                    CohortId = 5,
                    AccountId = 5,
                    ProviderId = 1,
                    LegalEntityName = "1_Employer5",
                    NumberOfDraftApprentices = 300,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = Now.AddMinutes(20)//TODO this needs fixing properly - was 200
                }
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
