﻿using System;
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
        [TestCase("DateCreated", true, "5_Encoded", "6_Encoded")]
        public async Task Then_Sort_IsApplied_Correctly(string sortField, bool reverse, string expectedFirstId, string expectedLastId)
        {
            var fixture = new WhenMappingDraftRequestToViewModelFixture().WithSortApplied(sortField, reverse);
            await fixture.Map();

            fixture.Verify_Sort_IsApplied(expectedFirstId, expectedLastId);
        }
    }

    public class WhenMappingDraftRequestToViewModelFixture
    {
        private readonly Mock<IEncodingService> _encodingService;
        private readonly CohortsByProviderRequest _draftRequest;
        private readonly DraftRequestViewModelMapper _mapper;
        private DraftViewModel _draftViewModel;

        private const long ProviderId = 1;
        private readonly DateTime _now = DateTime.Now;

        public WhenMappingDraftRequestToViewModelFixture()
        {
            _encodingService = new Mock<IEncodingService>();
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

            _draftRequest = new CohortsByProviderRequest() { ProviderId = ProviderId };
            var getCohortsResponse = CreateGetCohortsResponse();

            commitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(getCohortsResponse);
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            var providerRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

            var pasAccountApiClient = new Mock<IPasAccountApiClient>();
            pasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            _mapper = new DraftRequestViewModelMapper(commitmentsApiClient.Object, providerRelationshipsApiClient.Object, urlHelper.Object, pasAccountApiClient.Object, _encodingService.Object);
        }

        public async Task<WhenMappingDraftRequestToViewModelFixture> Map()
        {
            _draftViewModel = await _mapper.Map(_draftRequest);
            return this;
        }

        public WhenMappingDraftRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            _draftRequest.SortField = sortField;
            _draftRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_OnlyTheCohorts_InDraftWithProvider_Are_Mapped()
        {
            Assert.AreEqual(2, _draftViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInReviewViewModel(5));
            Assert.IsNotNull(GetCohortInReviewViewModel(6));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

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
            Assert.AreEqual("Employer5", _draftViewModel.Cohorts.First().EmployerName);
            Assert.AreEqual("Employer6", _draftViewModel.Cohorts.Last().EmployerName);
        }

        public void Verify_DateCreated_Is_Mapped()
        {
            Assert.AreEqual(_now.AddMinutes(-2), GetCohortInReviewViewModel(5).DateCreated);
            Assert.AreEqual(_now.AddMinutes(-1), GetCohortInReviewViewModel(6).DateCreated);
        }

        public void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(ProviderId, _draftViewModel.ProviderId);
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.AreEqual(firstId, _draftViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual(lastId, _draftViewModel.Cohorts.Last().CohortReference);
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
                    CreatedOn = _now.AddMinutes(-4)
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
                    CreatedOn = _now.AddMinutes(-3)
                },
                 new()
                 {
                     CohortId = 5,
                     AccountId = 5,
                     ProviderId = 1,
                     LegalEntityName = "Employer5",
                     NumberOfDraftApprentices = 500,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = _now.AddMinutes(-2)
                 },
                 new()
                 {
                     CohortId = 6,
                     AccountId = 6,
                     ProviderId = 1,
                     LegalEntityName = "Employer6",
                     NumberOfDraftApprentices = 600,
                     IsDraft = true,
                     WithParty = Party.Provider,
                     CreatedOn = _now.AddMinutes(-1)
                 }
            };

            return new GetCohortsResponse(cohorts);
        }

        private static long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private DraftCohortSummaryViewModel GetCohortInReviewViewModel(long id)
        {
            return _draftViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
