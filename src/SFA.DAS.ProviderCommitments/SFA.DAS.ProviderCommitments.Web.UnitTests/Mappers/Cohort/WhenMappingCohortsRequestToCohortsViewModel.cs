using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WhenMappingCohortsRequestToCohortsViewModel
    {
        [Test]
        public async Task TheCohortsInDraftIsPopulatedCorrectly()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture();
            var result = await f.Sut.Map(f.CohortsRequest);

            f.VerifyCohortsInDraftIsCorrect(result);
        }

        [Test]
        public async Task TheCohortsInReviewIsPopulatedCorrectly()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture();
            var result = await f.Sut.Map(f.CohortsRequest);

            f.VerifyCohortsInReviewIsCorrect(result);
        }

        [Test]
        public async Task TheCohortsWithEmployerIsPopulatedCorrectly()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture();
            var result = await f.Sut.Map(f.CohortsRequest);

            f.VerifyCohortsWithEmployerIsCorrect(result);
        }

        [Test]
        public async Task TheCohortsWithTransferSenderIsPopulatedCorrectly()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture();
            var result = await f.Sut.Map(f.CohortsRequest);

            f.VerifyCohortsWithTransferSenderIsCorrect(result);
        }

        [Test]
        public async Task WhenNoCohortsAreNotFoundThereAreNoDrilldownLinks()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture().WithNoCohortsFound();
            var result = await f.Sut.Map(f.CohortsRequest);

            f.VerifyNoDrillDownLinks(result);
        }

        [Test]
        public async Task WhenThereAreNoCreatePermissionsGrantedToThisProviderTheShowDraftsBingoBoxIsNotDisplayed()
        {
            var f = new WhenMappingCohortsRequestToCohortsViewModelFixture().WithCreatePermissionsForAnEmployer();
            var result = await f.Sut.Map(f.CohortsRequest);

            Assert.IsTrue(result.ShowDrafts);
        }

        public class WhenMappingCohortsRequestToCohortsViewModelFixture
        {
            public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; }
            public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; }
            public Mock<IUrlHelper> UrlHelper { get; }
            public CohortsByProviderRequest CohortsRequest { get; }
            public CohortsSummaryViewModelMapper Sut { get; }

            private Fixture _fixture;

            public WhenMappingCohortsRequestToCohortsViewModelFixture()
            {
                _fixture = new Fixture();
                CohortsRequest = _fixture.Create<CohortsByProviderRequest>();

                CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
                CommitmentsApiClient.Setup(x => x.GetCohorts(It.IsAny<GetCohortsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(CreateGetCohortsResponse());
                ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

                UrlHelper = new Mock<IUrlHelper>();
                UrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

                Sut = new CohortsSummaryViewModelMapper(CommitmentsApiClient.Object, ProviderRelationshipsApiClient.Object, UrlHelper.Object);
            }

            public WhenMappingCohortsRequestToCohortsViewModelFixture WithNoCohortsFound()
            {
                CommitmentsApiClient.Setup(x => x.GetCohorts(It.IsAny<GetCohortsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetCohortsResponse(new List<CohortSummary>()));
                return this;
            }
            
            public WhenMappingCohortsRequestToCohortsViewModelFixture WithCreatePermissionsForAnEmployer()
            {
                ProviderRelationshipsApiClient.Setup(x =>
                    x.HasRelationshipWithPermission(
                        It.Is<HasRelationshipWithPermissionRequest>(p =>
                            p.Ukprn == CohortsRequest.ProviderId && p.Operation == Operation.CreateCohort),
                        It.IsAny<CancellationToken>())).ReturnsAsync(true);
                return this;
            }

            public void VerifyCohortsInDraftIsCorrect(CohortsViewModel result)
            {
                Assert.IsNotNull(result.CohortsInDraft);
                Assert.AreEqual(5, result.CohortsInDraft.Count);
                Assert.AreEqual("drafts", result.CohortsInDraft.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Draft")));
            }

            public void VerifyCohortsInReviewIsCorrect(CohortsViewModel result)
            {
                Assert.IsNotNull(result.CohortsInReview);
                Assert.AreEqual(4, result.CohortsInReview.Count);
                Assert.AreEqual("ready to review", result.CohortsInReview.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Review")));
            }

            public void VerifyCohortsWithEmployerIsCorrect(CohortsViewModel result)
            {
                Assert.IsNotNull(result.CohortsWithEmployer);
                Assert.AreEqual(3, result.CohortsWithEmployer.Count);
                Assert.AreEqual("with employers", result.CohortsWithEmployer.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithEmployer")));
            }

            public void VerifyCohortsWithTransferSenderIsCorrect(CohortsViewModel result)
            {
                Assert.IsNotNull(result.CohortsWithTransferSender);
                Assert.AreEqual(2, result.CohortsWithTransferSender.Count);
                Assert.AreEqual("with transfer sending employers", result.CohortsWithTransferSender.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithTransferSender")));
            }

            public void VerifyNoDrillDownLinks(CohortsViewModel result)
            {
                Assert.IsNull(result.CohortsInDraft.Url);
                Assert.IsNull(result.CohortsInReview.Url);
                Assert.IsNull(result.CohortsWithEmployer.Url);
                Assert.IsNull(result.CohortsWithTransferSender.Url);
            }

            private static void PopulateWith(List<CohortSummary> list, bool draft, Party withParty)
            {
                foreach (var item in list)
                {
                    item.IsDraft = draft;
                    item.WithParty = withParty;
                }
            }

            private GetCohortsResponse CreateGetCohortsResponse()
            {
                var listInDraft = _fixture.CreateMany<CohortSummary>(5).ToList();
                PopulateWith(listInDraft, true, Party.Provider);

                var listInReview = _fixture.CreateMany<CohortSummary>(4).ToList();
                PopulateWith(listInReview, false, Party.Provider);

                var listWithEmployer = _fixture.CreateMany<CohortSummary>(3).ToList();
                PopulateWith(listWithEmployer, false, Party.Employer);

                var listWithTransferSender = _fixture.CreateMany<CohortSummary>(2).ToList();
                PopulateWith(listWithTransferSender, false, Party.TransferSender);

                var cohorts = listInDraft.Union(listInReview).Union(listWithEmployer).Union(listWithTransferSender);

                return new GetCohortsResponse(cohorts);
            }
        }
    }
}
