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
        private readonly Mock<IEncodingService> _encodingService;
        private readonly CohortsByProviderRequest _reviewRequest;
        private readonly ReviewRequestViewModelMapper _mapper;
        private ReviewViewModel _reviewViewModel;

        private const long ProviderId = 1;
        private readonly DateTime _now = DateTime.Now;

        public WhenMappingReviewRequestToViewModelFixture()
        {
            _encodingService = new Mock<IEncodingService>();
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

            _reviewRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            var getCohortsResponse = CreateGetCohortsResponse();

            commitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(getCohortsResponse);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");
          
            var providerRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

            var pasAccountApiClient = new Mock<IPasAccountApiClient>();
            pasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            _mapper = new ReviewRequestViewModelMapper(commitmentsApiClient.Object, providerRelationshipsApiClient.Object, urlHelper.Object, pasAccountApiClient.Object, _encodingService.Object);
        }

        public async Task<WhenMappingReviewRequestToViewModelFixture> Map()
        {
            _reviewViewModel = await _mapper.Map(_reviewRequest);
            return this;
        }

        public WhenMappingReviewRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            _reviewRequest.SortField = sortField;
            _reviewRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_OnlyTheCohorts_ReadyForReviewForProvider_Are_Mapped()
        {
            Assert.AreEqual(3, _reviewViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(1));
            Assert.IsNotNull(GetCohortInReviewViewModel(2));
            Assert.IsNotNull(GetCohortInReviewViewModel(5));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(3));

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
            Assert.AreEqual(_now.AddMinutes(-10), GetCohortInReviewViewModel(1).DateReceived);
            Assert.AreEqual(_now.AddMinutes(-2), GetCohortInReviewViewModel(2).DateReceived);
        }

        public void Verify_Ordered_By_DateCreatedDescending()
        {
            Assert.AreEqual("1_Employer5", _reviewViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer2", _reviewViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, _reviewViewModel.ProviderId);
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.AreEqual(firstId, _reviewViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual(lastId, _reviewViewModel.Cohorts.Last().CohortReference);
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
                    CreatedOn = _now.AddMinutes(-10)
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
                    CreatedOn = _now.AddMinutes(-5),
                    LatestMessageFromEmployer = new Message("This is latestMessage from employer", _now.AddMinutes(-2))
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
                    CreatedOn = _now.AddMinutes(-1)
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
                    CreatedOn = _now
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
                    CreatedOn = _now.AddMinutes(20)//TODO this needs fixing properly - was 200
                }
            };

            return new GetCohortsResponse(cohorts);
        }

        private static long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private ReviewCohortSummaryViewModel GetCohortInReviewViewModel(long id)
        {
            return _reviewViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
