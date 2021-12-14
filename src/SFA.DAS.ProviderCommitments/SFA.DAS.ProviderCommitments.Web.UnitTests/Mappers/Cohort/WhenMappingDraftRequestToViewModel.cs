using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
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
    public class WhenMappingDraftRequestToViewModel
    {
        [Test]
        public async Task OnlyTheCohortsInDraftWithProviderAreMapped()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_OnlyTheCohorts_InDraftWithProvider_Are_Mapped();
        }

        [Test]
        public async Task Then_TheCohortReferenceIsMapped()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public async Task Then_EmployerNameIsMapped()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_EmployerName_Is_Mapped();
        }

        [Test]
        public async Task Then_NumberOfApprenticesAreMapped()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_DateCreatedAscending();
        }

        [Test]
        public async Task Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }

        [Test]
        public async Task Then_DateCreated_IsMapped_Correctly()
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture();
            await fixture.Map();

            fixture.Verify_DateCreated_Is_Mapped();
        }

        [TestCase("", false, "5_Encoded", "6_Encoded")]
        [TestCase("Employer", false, "5_Encoded", "6_Encoded")]
        [TestCase("Employer", true, "6_Encoded", "5_Encoded")]
        [TestCase("CohortReference", false, "5_Encoded", "6_Encoded")]
        [TestCase("CohortReference", true, "6_Encoded", "5_Encoded")]
        [TestCase("DateCreated", false, "5_Encoded", "6_Encoded")]
        [TestCase("DateCreated", true, "6_Encoded", "5_Encoded")]
        public async Task Then_Sort_IsApplied_Correctly(string sortField, bool reverse, string expectedFirstId, string expectedLastId)
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture().WithSortApplied(sortField, reverse);
            await fixture.Map();

            fixture.Verify_Sort_IsApplied(expectedFirstId, expectedLastId);
        }
    }

    public class WhenMappingDraftRequestToViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; }
        public Mock<IPasAccountApiClient> PasAccountApiClient { get; set; }
        public Mock<IUrlHelper> UrlHelper { get; }
        public CohortsByProviderRequest DraftRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public DraftRequestViewModelMapper Mapper { get; set; }
        public DraftViewModel DraftViewModel { get; set; }

        public long ProviderId => 1;
        public DateTime Now = DateTime.Now;

        public WhenMappingDraftRequestToViewModelFixture()
        {
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            DraftRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

            PasAccountApiClient = new Mock<IPasAccountApiClient>();
            PasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            UrlHelper = new Mock<IUrlHelper>();
            UrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            Mapper = new DraftRequestViewModelMapper(CommitmentsApiClient.Object, ProviderRelationshipsApiClient.Object, UrlHelper.Object, PasAccountApiClient.Object, EncodingService.Object);
        }

        public async Task<WhenMappingDraftRequestToViewModelFixture> Map()
        {
            DraftViewModel = await Mapper.Map(DraftRequest);
            return this;
        }

        public WhenMappingDraftRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            DraftRequest.SortField = sortField;
            DraftRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_OnlyTheCohorts_InDraftWithProvider_Are_Mapped()
        {
            Assert.AreEqual(2, DraftViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(5));
            Assert.IsNotNull(GetCohortInReviewViewModel(6));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

            Assert.AreEqual("5_Encoded", GetCohortInReviewViewModel(5).CohortReference);
            Assert.AreEqual("6_Encoded", GetCohortInReviewViewModel(6).CohortReference);
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.AreEqual("Employer5", GetCohortInReviewViewModel(5).EmployerName);
            Assert.AreEqual("Employer6", GetCohortInReviewViewModel(6).EmployerName);
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(500, GetCohortInReviewViewModel(5).NumberOfApprentices);
            Assert.AreEqual(600, GetCohortInReviewViewModel(6).NumberOfApprentices);
        }

        public void Verify_Ordered_By_DateCreatedAscending()
        {
            Assert.AreEqual("Employer5", DraftViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer6", DraftViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_DateCreated_Is_Mapped()
        {
            Assert.AreEqual(Now.AddMinutes(-3), GetCohortInReviewViewModel(5).DateCreated);
            Assert.AreEqual(Now.AddMinutes(-3), GetCohortInReviewViewModel(6).DateCreated);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, DraftViewModel.ProviderId);
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.AreEqual(firstId, DraftViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual(lastId, DraftViewModel.Cohorts.Last().CohortReference);
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
                    CreatedOn = Now.AddMinutes(-10)
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
                    CreatedOn = Now.AddMinutes(-5),
                    LatestMessageFromEmployer = new Message("This is latestMessage from employer", Now.AddMinutes(-2))
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
                    CreatedOn = Now.AddMinutes(-4)
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
                    CreatedOn = Now.AddMinutes(-3)
                },
                 new CohortSummary
                 {
                     CohortId = 5,
                     AccountId = 5,
                     ProviderId = 1,
                     LegalEntityName = "Employer5",
                     NumberOfDraftApprentices = 500,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = Now.AddMinutes(-2)
                 },
                 new CohortSummary
                 {
                     CohortId = 6,
                     AccountId = 6,
                     ProviderId = 1,
                     LegalEntityName = "Employer6",
                     NumberOfDraftApprentices = 600,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = Now.AddMinutes(-1)
                 }
            };

            return new GetCohortsResponse(cohorts);
        }

        private long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private DraftCohortSummaryViewModel GetCohortInReviewViewModel(long id)
        {
            return DraftViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
