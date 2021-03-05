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
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_Correctly();
        }

        [Test]
        public void Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingWithEmployerToViewModelFixture();
            _ = fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }
    }

    public class WhenMappingWithEmployerToViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public CohortsByProviderRequest WithEmployerRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public WithEmployerViewModelMapper Mapper { get; set; }
        public WithEmployerViewModel WithEmployerViewModel { get; set; }

        public long ProviderId => 1;

        public WhenMappingWithEmployerToViewModelFixture()
        {
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            WithEmployerRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            Mapper = new WithEmployerViewModelMapper(CommitmentsApiClient.Object, EncodingService.Object);
        }

        public async Task<WhenMappingWithEmployerToViewModelFixture> Map()
        {
            WithEmployerViewModel = await Mapper.Map(WithEmployerRequest);
            return this;
        }

        public void Verify_OnlyTheCohorts_WithEmployer_Are_Mapped()
        {
            Assert.AreEqual(2, WithEmployerViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortsWithEmployer(1));
            Assert.IsNotNull(GetCohortsWithEmployer(2));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

            Assert.AreEqual("1_Encoded", GetCohortsWithEmployer(1).CohortReference);
            Assert.AreEqual("2_Encoded", GetCohortsWithEmployer(2).CohortReference);
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.AreEqual("Employer1", GetCohortsWithEmployer(1).EmployerName);
            Assert.AreEqual("Employer2", GetCohortsWithEmployer(2).EmployerName);
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(100, GetCohortsWithEmployer(1).NumberOfApprentices);
            Assert.AreEqual(200, GetCohortsWithEmployer(2).NumberOfApprentices);
        }

        public void Verify_LastMessage_Is_MappedCorrectly()
        {
            Assert.AreEqual("This is latestMessage from Provider", GetCohortsWithEmployer(1).LastMessage);
            Assert.AreEqual("No message added", GetCohortsWithEmployer(2).LastMessage);
        }

        public void Verify_Ordered_By_Correctly()
        {
            Assert.AreEqual("Employer1", WithEmployerViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer2", WithEmployerViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, WithEmployerViewModel.ProviderId);
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
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddMinutes(-1),
                    LatestMessageFromProvider = new Message("This is latestMessage from Provider", DateTime.Now.AddMinutes(-5))
                },
                new CohortSummary
                {
                    CohortId = 2,
                    AccountId = 2,
                    ProviderId = 1,
                    LegalEntityName = "Employer2",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddMinutes(-3),
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
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now.AddMinutes(-1)
                },
                 new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 4,
                    ProviderId = 1,
                    LegalEntityName = "Employer4",
                    NumberOfDraftApprentices = 400,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }

        private long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private WithEmployerSummaryViewModel GetCohortsWithEmployer(long id)
        {
            return WithEmployerViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
