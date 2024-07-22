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
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingWithEmployerToViewModel
    {
        [Test]
        public async Task OnlyTheCohortsReadyForWithEmployerAreMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_OnlyTheCohorts_WithEmployer_Are_Mapped();
        }

        [Test]
        public async Task Then_TheCohortReferenceIsMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public async Task Then_EmployerNameIsMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_EmployerName_Is_Mapped();
        }

        [Test]
        public async Task Then_NumberOfApprenticesAreMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public async Task Then_LastMessage_IsMapped_Correctly()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_LastMessage_Is_MappedCorrectly();
        }

        [Test]
        public async Task Then_DateSentToEmployer_IsMapped_Correctly()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_DateSentToEmployer_Is_Mapped();
        }

        [Test]
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_Correctly();
        }

        [Test]
        public async Task Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }

        [TestCase("", false, "5_Encoded", "2_Encoded")]
        [TestCase("Employer", false, "5_Encoded", "2_Encoded")]
        [TestCase("Employer", true, "2_Encoded", "5_Encoded")]
        [TestCase("CohortReference", false, "1_Encoded", "5_Encoded")]
        [TestCase("CohortReference", true, "5_Encoded", "1_Encoded")]
        [TestCase("DateSentToEmployer", false, "5_Encoded", "2_Encoded")]
        [TestCase("DateSentToEmployer", true, "5_Encoded", "2_Encoded")]
        public async Task Then_Sort_IsApplied_Correctly(string sortField, bool reverse, string expectedFirstId, string expectedLastId)
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture().WithSortApplied(sortField, reverse);
            await fixture.Map();

            fixture.Verify_Sort_IsApplied(expectedFirstId, expectedLastId);
        }
    }

    public class WhenMappingWithEmployerToViewModelFixture
    {
        private readonly Mock<IEncodingService> _encodingService;
        private readonly CohortsByProviderRequest _withEmployerRequest;
        private readonly WithEmployerViewModelMapper _mapper;
        private WithEmployerViewModel _withEmployerViewModel;

        private const long ProviderId = 1;
        private readonly DateTime _now = DateTime.Now;

        public WhenMappingWithEmployerToViewModelFixture()
        {
            _encodingService = new Mock<IEncodingService>();
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

            _withEmployerRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            var getCohortsResponse = CreateGetCohortsResponse();

            commitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(getCohortsResponse);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            var approvalsOuterApiClient = new Mock<IApprovalsOuterApiClient>();

            var pasAccountApiClient = new Mock<IPasAccountApiClient>();
            pasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            _mapper = new WithEmployerViewModelMapper(commitmentsApiClient.Object, approvalsOuterApiClient.Object, urlHelper.Object, pasAccountApiClient.Object, _encodingService.Object);
        }

        public async Task<WhenMappingWithEmployerToViewModelFixture> Map()
        {
            _withEmployerViewModel = await _mapper.Map(_withEmployerRequest);
            return this;
        }

        public WhenMappingWithEmployerToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            _withEmployerRequest.SortField = sortField;
            _withEmployerRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_OnlyTheCohorts_WithEmployer_Are_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_withEmployerViewModel.Cohorts.Count(), Is.EqualTo(3));
                Assert.That(GetCohortsWithEmployer(1), Is.Not.Null);
                Assert.That(GetCohortsWithEmployer(2), Is.Not.Null);
                Assert.That(GetCohortsWithEmployer(5), Is.Not.Null);
            });
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(3));

            Assert.Multiple(() =>
            {
                Assert.That(GetCohortsWithEmployer(1).CohortReference, Is.EqualTo("1_Encoded"));
                Assert.That(GetCohortsWithEmployer(2).CohortReference, Is.EqualTo("2_Encoded"));
                Assert.That(GetCohortsWithEmployer(5).CohortReference, Is.EqualTo("5_Encoded"));
            });
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GetCohortsWithEmployer(1).EmployerName, Is.EqualTo("Employer1"));
                Assert.That(GetCohortsWithEmployer(2).EmployerName, Is.EqualTo("Employer2"));
            });
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GetCohortsWithEmployer(1).NumberOfApprentices, Is.EqualTo(100));
                Assert.That(GetCohortsWithEmployer(2).NumberOfApprentices, Is.EqualTo(200));
            });
        }

        public void Verify_LastMessage_Is_MappedCorrectly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GetCohortsWithEmployer(1).LastMessage, Is.EqualTo("This is latestMessage from Provider"));
                Assert.That(GetCohortsWithEmployer(2).LastMessage, Is.EqualTo("No message added"));
            });
        }

        public void Verify_Ordered_By_Correctly()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_withEmployerViewModel.Cohorts.First().EmployerName, Is.EqualTo("1_Employer5"));
                Assert.That(_withEmployerViewModel.Cohorts.Last().EmployerName, Is.EqualTo("Employer2"));
            });
        }

        public void Verify_DateSentToEmployer_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GetCohortsWithEmployer(1).DateSentToEmployer, Is.EqualTo(_now.AddMinutes(-5)));
                Assert.That(GetCohortsWithEmployer(2).DateSentToEmployer, Is.EqualTo(_now.AddMinutes(-3)));
            });
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.That(_withEmployerViewModel.ProviderId, Is.EqualTo(ProviderId));
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_withEmployerViewModel.Cohorts.First().CohortReference, Is.EqualTo(firstId));
                Assert.That(_withEmployerViewModel.Cohorts.Last().CohortReference, Is.EqualTo(lastId));
            });
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
                    WithParty = Party.Employer,
                    CreatedOn = _now.AddMinutes(-1),
                    LatestMessageFromProvider = new Message("This is latestMessage from Provider", _now.AddMinutes(-5))
                },
                new()
                {
                    CohortId = 2,
                    AccountId = 2,
                    ProviderId = 1,
                    LegalEntityName = "Employer2",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = _now.AddMinutes(-3),
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
                    WithParty = Party.Provider,
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
                    WithParty = Party.Provider,
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
                    WithParty = Party.Employer,
                    CreatedOn = _now.AddMinutes(20)//TODO this needs fixing properly - was 200
                }
            };

            return new GetCohortsResponse(cohorts);
        }

        private static long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private WithEmployerSummaryViewModel GetCohortsWithEmployer(long id)
        {
            return _withEmployerViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
