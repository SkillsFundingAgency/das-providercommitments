using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModel
    {

        [Test]
        public async Task Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture();
            await fixture.WithCohort().SetUp().Map();

            fixture.Verify_ProviderId_IsMapped();
        }

        [Test]
        public async Task Then_HasCreateCohortPermission_IsMapped()
        {
            var fixture = new WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture();
            await fixture.WithCohort().SetUp().Map();

            fixture.Verify_HasCreateCohortPermission_IsMapped();
        }

        [Test]
        public async Task Then_HasExistingCohort_IsMapped()
        {
            var fixture = new WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture();
            await fixture.WithCohort().SetUp().Map();

            fixture.Verify_HasExistingCohort_IsMapped();
        }

        [Test]
        public async Task Then_WhenNoCohortExist_HasExistingCohort_IsMapped_Correctly_()
        {
            var fixture = new WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture();
            await fixture.WithNoCohort().SetUp().Map();

            fixture.Verify_WhenNoCohortExist_HasExistingCohort_IsMapped();
        }

        [Test]
        public async Task Then_WhenNoCohortExistInReviewOrDraftStatus_HasExistingCohort_IsMapped_Correctly_()
        {
            var fixture = new WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture();
            await fixture.WithNoDraftOrReviewCohort().SetUp().Map();

            fixture.Verify_WhenNoCohortExist_HasExistingCohort_IsMapped();
        }
    }

    public class WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture
    {
        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private readonly Mock<IProviderRelationshipsApiClient> _providerRelationshipsApiClient;
        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        private SelectAddDraftApprenticeshipJourneyViewModel _viewModel;
        private SelectAddDraftApprenticeshipJourneyRequestViewModelMapper _mapper;
        private readonly bool _hasCreateCohortPermission;

        private const long ProviderId = 1;
        private readonly DateTime _now = DateTime.Now;
        
        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture()
        {
            _hasCreateCohortPermission = true;

            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };

            _providerRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();
            _providerRelationshipsApiClient
                .Setup(p => p.HasRelationshipWithPermission(It.IsAny<HasRelationshipWithPermissionRequest>(), CancellationToken.None))
                .ReturnsAsync(_hasCreateCohortPermission);
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture SetUp()
        {
            _mapper = new SelectAddDraftApprenticeshipJourneyRequestViewModelMapper(_commitmentsApiClient.Object, _providerRelationshipsApiClient.Object);
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithCohort()
        {
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient
                .Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None))
                .ReturnsAsync(new GetCohortsResponse(CreateGetCohortsResponse()));
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithNoCohort()
        {
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient
                .Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None))
                .ReturnsAsync(new GetCohortsResponse(new List<CohortSummary>()));
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithNoDraftOrReviewCohort()
        {
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient
                .Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None))
                .ReturnsAsync(new GetCohortsResponse(new List<CohortSummary>
            {
                new()
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.TransferSender,
                    CreatedOn = _now.AddMinutes(-10)
                },
                new()
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = _now.AddMinutes(-10)
                }
            }));
            return this;
        }

        public async Task<WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture> Map()
        {
            _viewModel = await _mapper.Map(_request);
            return this;
        }

        private IEnumerable<CohortSummary> CreateGetCohortsResponse()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>
            {
                new()
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = true,
                    WithParty = Party.Provider,
                    CreatedOn = _now.AddMinutes(-10)
                },
                new()
                {
                    CohortId = 2,
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
                    CohortId = 3,
                    AccountId = 1,
                    ProviderId = 1,
                    LegalEntityName = "Employer1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = _now.AddMinutes(-10)
                }
            };

            return cohorts;
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, _viewModel.ProviderId);
        }

        public void Verify_HasCreateCohortPermission_IsMapped()
        {
            Assert.AreEqual(_hasCreateCohortPermission, _viewModel.HasCreateCohortPermission);
        }

        public void Verify_HasExistingCohort_IsMapped()
        {
            Assert.AreEqual(true, _viewModel.HasExistingCohort);
        }

        public void Verify_WhenNoCohortExist_HasExistingCohort_IsMapped()
        {
            Assert.AreEqual(false, _viewModel.HasExistingCohort);
        }
    }
}
