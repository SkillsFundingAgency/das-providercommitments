﻿using System;
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
    public class WhenMappingChooseCohortViewModel
    {
        [Test]
        public async Task CohortsInDraftWithProviderAreMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_Cohorts_InDraftWithProvider_Are_Mapped();
        }

        [Test]
        public async Task CohortsInReviewWithProviderAreMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_Cohorts_InReviewWithProvider_Are_Mapped();
        }

        [Test]
        public async Task Then_TheCohortReferenceIsMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public async Task Then_EmployerNameIsMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_EmployerName_Is_Mapped();
        }

        [Test]
        public async Task Then_NumberOfApprenticesAreMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public void Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }

        [Test]
        public void Then_AccountLegalEntityPublicHashedId_IsMapped()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            fixture.Map();

            fixture.Verify_AccountLegalEntityPublicHashedId_IsMapped();
        }

        [Test]
        public async Task Then_Cohort_OrderBy_OnDateCreated_Correctly()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_Ordered_By_DateCreatedAscending();
        }

        [Test]
        public async Task Then_Cohort_Ordered_By_EmployerNameDescending_Correctly()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();

            fixture.ChooseCohortByProviderRequest.SortField = nameof(ChooseCohortSummaryViewModel.EmployerName);
            fixture.ChooseCohortByProviderRequest.ReverseSort = true;

            await fixture.Map();

            fixture.Verify_Ordered_By_EmployerNameDescending();
        }

        [Test]
        public async Task Then_Cohort_Ordered_By_CohortReferenceDescending_Correctly()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();

            fixture.ChooseCohortByProviderRequest.SortField = nameof(ChooseCohortSummaryViewModel.CohortReference);
            fixture.ChooseCohortByProviderRequest.ReverseSort = true;

            await fixture.Map();

            fixture.Verify_Ordered_By_CohortReferenceDescending();
        }

        [Test]
        public async Task Then_Cohort_Ordered_By_StatusDescending_Correctly()
        {
            var fixture = new WhenMappingChooseCohortViewModelFixture();

            fixture.ChooseCohortByProviderRequest.SortField = nameof(ChooseCohortSummaryViewModel.Status);
            fixture.ChooseCohortByProviderRequest.ReverseSort = true;

            await fixture.Map();

            fixture.Verify_Ordered_By_StatusDescending();
        }
    }

    public class WhenMappingChooseCohortViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public ChooseCohortByProviderRequest ChooseCohortByProviderRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public ChooseCohortViewModelMapper Mapper { get; set; }
        public ChooseCohortViewModel ChooseCohortViewModel { get; set; }

        public long ProviderId => 1;

        public WhenMappingChooseCohortViewModelFixture()
        {
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            ChooseCohortByProviderRequest = new ChooseCohortByProviderRequest() { ProviderId = ProviderId };
            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            Mapper = new ChooseCohortViewModelMapper(CommitmentsApiClient.Object, EncodingService.Object);
        }

        public async Task<WhenMappingChooseCohortViewModelFixture> Map()
        {
            ChooseCohortViewModel = await Mapper.Map(ChooseCohortByProviderRequest);
            return this;
        }

        public void Verify_Cohorts_InDraftWithProvider_Are_Mapped()
        {
            Assert.AreEqual(4, ChooseCohortViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(5));
            Assert.IsNotNull(GetCohortInReviewViewModel(6));
        }

        public void Verify_Cohorts_InReviewWithProvider_Are_Mapped()
        {
            Assert.AreEqual(4, ChooseCohortViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(1));
            Assert.IsNotNull(GetCohortInReviewViewModel(2));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(4));


            Assert.AreEqual("1_Encoded", GetCohortInReviewViewModel(1).CohortReference);
            Assert.AreEqual("2_Encoded", GetCohortInReviewViewModel(2).CohortReference);
            Assert.AreEqual("5_Encoded", GetCohortInReviewViewModel(5).CohortReference);
            Assert.AreEqual("6_Encoded", GetCohortInReviewViewModel(6).CohortReference);
        }

        public void Verify_EmployerName_Is_Mapped()
        {
            Assert.AreEqual("Employer1", GetCohortInReviewViewModel(1).EmployerName);
            Assert.AreEqual("Employer2", GetCohortInReviewViewModel(2).EmployerName);
            Assert.AreEqual("Employer5", GetCohortInReviewViewModel(5).EmployerName);
            Assert.AreEqual("Employer6", GetCohortInReviewViewModel(6).EmployerName);
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(500, GetCohortInReviewViewModel(5).NumberOfApprentices);
            Assert.AreEqual(600, GetCohortInReviewViewModel(6).NumberOfApprentices);
        }
        public void Verify_AccountLegalEntityPublicHashedId_IsMapped()
        {
            Assert.AreEqual("100A", GetCohortInReviewViewModel(1).AccountLegalEntityPublicHashedId);
            Assert.AreEqual("200B", GetCohortInReviewViewModel(2).AccountLegalEntityPublicHashedId);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, ChooseCohortViewModel.ProviderId);
        }

        public void Verify_Ordered_By_DateCreatedAscending()
        {
            Assert.AreEqual("Employer1", ChooseCohortViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer6", ChooseCohortViewModel.Cohorts.Last().EmployerName);
        }
        public void Verify_Ordered_By_EmployerNameDescending()
        {
            Assert.AreEqual("Employer6", ChooseCohortViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer1", ChooseCohortViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_Ordered_By_CohortReferenceDescending()
        {
            Assert.AreEqual("6_Encoded", ChooseCohortViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual("1_Encoded", ChooseCohortViewModel.Cohorts.Last().CohortReference);
        }

        public void Verify_Ordered_By_StatusDescending()
        {
            Assert.AreEqual("Ready to review", ChooseCohortViewModel.Cohorts.First().Status);
            Assert.AreEqual("Draft", ChooseCohortViewModel.Cohorts.Last().Status);
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
                    AccountLegalEntityPublicHashedId = "100A",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now.AddHours(-6),
                },
                new CohortSummary
                {
                    CohortId = 2,
                    AccountId = 2,
                    ProviderId = 1,
                    LegalEntityName = "Employer2",
                    AccountLegalEntityPublicHashedId = "200B",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.Provider,
                    CreatedOn = DateTime.Now.AddHours(-5)
                },
                new CohortSummary
                {
                    CohortId = 3,
                    AccountId = 3,
                    ProviderId = 1,
                    LegalEntityName = "Employer3",
                    AccountLegalEntityPublicHashedId = "300C",
                    NumberOfDraftApprentices = 300,
                    IsDraft = true,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddHours(-4)
                },
                 new CohortSummary
                {
                    CohortId = 4,
                    AccountId = 4,
                    ProviderId = 1,
                    LegalEntityName = "Employer4",
                    AccountLegalEntityPublicHashedId = "400D",
                    NumberOfDraftApprentices = 400,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = DateTime.Now.AddHours(-3)
                },
                 new CohortSummary
                 {
                     CohortId = 5,
                     AccountId = 5,
                     ProviderId = 1,
                     LegalEntityName = "Employer5",
                     AccountLegalEntityPublicHashedId = "500E",
                     NumberOfDraftApprentices = 500,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = DateTime.Now.AddHours(-2)
                 },
                 new CohortSummary
                 {
                     CohortId = 6,
                     AccountId = 6,
                     ProviderId = 1,
                     LegalEntityName = "Employer6",
                     AccountLegalEntityPublicHashedId = "600F",
                     NumberOfDraftApprentices = 600,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = DateTime.Now.AddHours(-1)
                 }
            };

            return new GetCohortsResponse(cohorts);
        }

        private long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private ChooseCohortSummaryViewModel GetCohortInReviewViewModel(long id)
        {
            return ChooseCohortViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}