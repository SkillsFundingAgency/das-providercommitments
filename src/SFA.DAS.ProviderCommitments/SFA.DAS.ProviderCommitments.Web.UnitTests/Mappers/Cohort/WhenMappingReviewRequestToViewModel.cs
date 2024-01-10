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
            Assert.That(_reviewViewModel.Cohorts.Count(), Is.EqualTo(3));

            Assert.That(GetCohortInReviewViewModel(1), Is.Not.Null);
            Assert.That(GetCohortInReviewViewModel(2), Is.Not.Null);
            Assert.That(GetCohortInReviewViewModel(5), Is.Not.Null);
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(3));

            Assert.That(GetCohortInReviewViewModel(1).CohortReference, Is.EqualTo("1_Encoded"));
            Assert.That(GetCohortInReviewViewModel(2).CohortReference, Is.EqualTo("2_Encoded"));
            Assert.That(GetCohortInReviewViewModel(5).CohortReference, Is.EqualTo("5_Encoded"));
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.That(GetCohortInReviewViewModel(1).EmployerName, Is.EqualTo("Employer1"));
            Assert.That(GetCohortInReviewViewModel(2).EmployerName, Is.EqualTo("Employer2"));
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.That(GetCohortInReviewViewModel(1).NumberOfApprentices, Is.EqualTo(100));
            Assert.That(GetCohortInReviewViewModel(2).NumberOfApprentices, Is.EqualTo(200));
        }

        public void Verify_LastMessage_Is_MappedCorrectly()
        {
            Assert.That(GetCohortInReviewViewModel(1).LastMessage, Is.EqualTo("No message added"));
            Assert.That(GetCohortInReviewViewModel(2).LastMessage, Is.EqualTo("This is latestMessage from employer"));
        }

        public void Verify_DateReceived_Is_Mapped()
        {
            Assert.That(GetCohortInReviewViewModel(1).DateReceived, Is.EqualTo(_now.AddMinutes(-10)));
            Assert.That(GetCohortInReviewViewModel(2).DateReceived, Is.EqualTo(_now.AddMinutes(-2)));
        }

        public void Verify_Ordered_By_DateCreatedDescending()
        {
            Assert.That(_reviewViewModel.Cohorts.First().EmployerName, Is.EqualTo("1_Employer5"));
            Assert.That(_reviewViewModel.Cohorts.Last().EmployerName, Is.EqualTo("Employer2"));
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.That(_reviewViewModel.ProviderId, Is.EqualTo(ProviderId));
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.That(_reviewViewModel.Cohorts.First().CohortReference, Is.EqualTo(firstId));
            Assert.That(_reviewViewModel.Cohorts.Last().CohortReference, Is.EqualTo(lastId));
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
