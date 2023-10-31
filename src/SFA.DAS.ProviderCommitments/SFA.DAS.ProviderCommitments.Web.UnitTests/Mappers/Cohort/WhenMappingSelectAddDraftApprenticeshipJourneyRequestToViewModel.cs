﻿using System;
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
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; }

        public SelectAddDraftApprenticeshipJourneyRequest Request { get; set; }
        public SelectAddDraftApprenticeshipJourneyViewModel ViewModel { get; set; }

        public SelectAddDraftApprenticeshipJourneyRequestViewModelMapper Mapper { get; set; }

        public GetCohortsResponse GetCohortsResponse { get; set; }

        public long ProviderId => 1;
        public DateTime Now = DateTime.Now;
        public bool HasCreateCohortPermission { get; set; }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture()
        {
            HasCreateCohortPermission = true;

            Request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };

            ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();
            ProviderRelationshipsApiClient
                .Setup(p => p.HasRelationshipWithPermission(It.IsAny<HasRelationshipWithPermissionRequest>(), CancellationToken.None))
                .ReturnsAsync(HasCreateCohortPermission);
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture SetUp()
        {
            Mapper = new SelectAddDraftApprenticeshipJourneyRequestViewModelMapper(CommitmentsApiClient.Object, ProviderRelationshipsApiClient.Object);
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithCohort()
        {
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient
                .Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None))
                .ReturnsAsync(new GetCohortsResponse(CreateGetCohortsResponse()));
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithNoCohort()
        {
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient
                .Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None))
                .ReturnsAsync(new GetCohortsResponse(new List<CohortSummary>()));
            return this;
        }

        public WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture WithNoDraftOrReviewCohort()
        {
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient
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
                    CreatedOn = Now.AddMinutes(-10)
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
                    CreatedOn = Now.AddMinutes(-10)
                }
            }));
            return this;
        }

        public async Task<WhenMappingSelectAddDraftApprenticeshipJourneyRequestToViewModelFixture> Map()
        {
            ViewModel = await Mapper.Map(Request);
            return this;
        }

        private IEnumerable<CohortSummary> CreateGetCohortsResponse()
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
                    IsDraft = true,
                    WithParty = Party.Provider,
                    CreatedOn = Now.AddMinutes(-10)
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
                    CreatedOn = Now.AddMinutes(-10)
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
                    CreatedOn = Now.AddMinutes(-10)
                }
            };

            return cohorts;
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, ViewModel.ProviderId);
        }

        public void Verify_HasCreateCohortPermission_IsMapped()
        {
            Assert.AreEqual(HasCreateCohortPermission, ViewModel.HasCreateCohortPermission);
        }

        public void Verify_HasExistingCohort_IsMapped()
        {
            Assert.AreEqual(true, ViewModel.HasExistingCohort);
        }

        public void Verify_WhenNoCohortExist_HasExistingCohort_IsMapped()
        {
            Assert.AreEqual(false, ViewModel.HasExistingCohort);
        }

    }
}
