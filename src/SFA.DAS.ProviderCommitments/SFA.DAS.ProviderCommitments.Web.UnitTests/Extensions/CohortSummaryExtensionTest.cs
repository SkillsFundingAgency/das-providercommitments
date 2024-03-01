using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]
    public class CohortSummaryExtensionTest
    {
        [Test]
        public void CohortSummary_GetStatus_Returns_Correct_Status_For_Review()
        {
            var cohortSummary = new CohortSummary
            {
                CohortId = 1,
                IsDraft = false,
                WithParty = Party.Provider
            };

            var status = cohortSummary.GetStatus();

            Assert.That(status, Is.EqualTo(CohortStatus.Review));
        }

        [Test]
        public void CohortSummary_GetStatus_Returns_Correct_Status_For_Draft()
        {
            var cohortSummary = new CohortSummary
            {
                CohortId = 1,
                IsDraft = true,
                WithParty = Party.Provider
            };

            var status = cohortSummary.GetStatus();

            Assert.That(status, Is.EqualTo(CohortStatus.Draft));
        }

        [Test]
        public void CohortSummary_GetStatus_Returns_Correct_Status_For_WithEmployer()
        {
            var cohortSummary = new CohortSummary
            {
                CohortId = 1,
                IsDraft = false,
                WithParty = Party.Employer
            };

            var status = cohortSummary.GetStatus();

            Assert.That(status, Is.EqualTo(CohortStatus.WithEmployer));
        }

        [Test]
        public void CohortSummary_GetStatus_Returns_Unknown_If_Unable_To_Find_The_Status()
        {
            var cohortSummary = new CohortSummary
            {
                CohortId = 1,
                IsDraft = true,
                WithParty = Party.Employer
            };

            var status = cohortSummary.GetStatus();

            Assert.That(status, Is.EqualTo(CohortStatus.Unknown));
        }

        [Test]
        public void TheCohortsInDraftIsPopulatedCorrectly()
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel();

            fixture.VerifyCohortsInDraftIsCorrect(result);
        }

        [Test]
        public void TheCohortsInReviewIsPopulatedCorrectly()
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel();

            fixture.VerifyCohortsInReviewIsCorrect(result);
        }

        [Test]
        public void TheCohortsWithEmployerIsPopulatedCorrectly()
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel();

            fixture.VerifyCohortsWithEmployerIsCorrect(result);
        }

        [Test]
        public void TheCohortsWithTransferSenderIsPopulatedCorrectly()
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel();

            fixture.VerifyCohortsWithTransferSenderIsCorrect(result);
        }

        [TestCase(CohortStatus.Draft)]
        [TestCase(CohortStatus.Review)]
        [TestCase(CohortStatus.WithEmployer)]
        [TestCase(CohortStatus.WithTransferSender)]
        public void TheCohortsStatusIsSetCorrectly(CohortStatus selectedCohortStatus)
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel(selectedCohortStatus);

            GetCohortCardLinkViewModelTestsFixture.VerifySelectedCohortStatusIsCorrect(result, selectedCohortStatus);
        }

        [TestCase(ProviderAgreementStatus.Agreed, true)]
        [TestCase(ProviderAgreementStatus.NotAgreed, false)]
        public void TheIsAgreementSignedIsPopulatedCorrectly(ProviderAgreementStatus providerAgreementStatus, bool expectedIsAgreementSigned)
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel(CohortStatus.Unknown, true, providerAgreementStatus);

            GetCohortCardLinkViewModelTestsFixture.VerifyIsSignedAgreementIsCorrect(result, expectedIsAgreementSigned);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TheShowDraftsIsPopulatedCorrectly(bool hasRelationship, bool expectedShowDrafts)
        {
            var fixture = new GetCohortCardLinkViewModelTestsFixture();
            var result = fixture.GetCohortCardLinkViewModel(CohortStatus.Unknown, hasRelationship);

            GetCohortCardLinkViewModelTestsFixture.VerifyShowDraftsIsCorrect(result, expectedShowDrafts);
        }

        private class GetCohortCardLinkViewModelTestsFixture
        {
            private readonly Fixture _fixture;
            private readonly Mock<IUrlHelper> _urlHelper;
            private readonly CohortSummary[] _cohortSummaries;
            private static long ProviderId => 10005654;

            public GetCohortCardLinkViewModelTestsFixture()
            {
                _urlHelper = new Mock<IUrlHelper>();
                _urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");
                _fixture = new Fixture();

                _cohortSummaries = CreateGetCohortsResponse();
            }

            public void VerifyCohortsInDraftIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.That(result.CohortsInDraft, Is.Not.Null);
                Assert.That(result.CohortsInDraft.Count, Is.EqualTo(5));
                Assert.That(result.CohortsInDraft.Description, Is.EqualTo("Drafts"));
                _urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Draft")));
            }

            public void VerifyCohortsInReviewIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.That(result.CohortsInReview, Is.Not.Null);
                Assert.That(result.CohortsInReview.Count, Is.EqualTo(4));
                Assert.That(result.CohortsInReview.Description, Is.EqualTo("Ready for review"));
                _urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Review")));
            }

            public void VerifyCohortsWithEmployerIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.That(result.CohortsWithEmployer, Is.Not.Null);
                Assert.That(result.CohortsWithEmployer.Count, Is.EqualTo(3));
                Assert.That(result.CohortsWithEmployer.Description, Is.EqualTo("With employers"));
                _urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithEmployer")));
            }

            public void VerifyCohortsWithTransferSenderIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.That(result.CohortsWithTransferSender, Is.Not.Null);
                Assert.That(result.CohortsWithTransferSender.Count, Is.EqualTo(2));
                Assert.That(result.CohortsWithTransferSender.Description, Is.EqualTo("With transfer sending employers"));
                _urlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithTransferSender")));
            }

            public static void VerifySelectedCohortStatusIsCorrect(ApprenticeshipRequestsHeaderViewModel result, CohortStatus expectedCohortStatus)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result.CohortsWithTransferSender.IsSelected, Is.EqualTo(expectedCohortStatus == CohortStatus.WithTransferSender));
                    Assert.That(result.CohortsInDraft.IsSelected, Is.EqualTo(expectedCohortStatus == CohortStatus.Draft));
                    Assert.That(result.CohortsInReview.IsSelected, Is.EqualTo(expectedCohortStatus == CohortStatus.Review));
                    Assert.That(result.CohortsWithEmployer.IsSelected, Is.EqualTo(expectedCohortStatus == CohortStatus.WithEmployer));
                });
            }

            public static void VerifyIsSignedAgreementIsCorrect(ApprenticeshipRequestsHeaderViewModel result, bool expectedIsAgreementSigned)
            {
                Assert.That(result.IsAgreementSigned, Is.EqualTo(expectedIsAgreementSigned));
            }

            public static void VerifyShowDraftsIsCorrect(ApprenticeshipRequestsHeaderViewModel result, bool expectedShowDrafts)
            {
                Assert.That(result.ShowDrafts, Is.EqualTo(expectedShowDrafts));
            }

            public ApprenticeshipRequestsHeaderViewModel GetCohortCardLinkViewModel(CohortStatus selectedCohortStatus = CohortStatus.Draft, bool hasRelationship = true, ProviderAgreementStatus providerAgreementStatus = ProviderAgreementStatus.Agreed)
            {
                return _cohortSummaries.GetCohortCardLinkViewModel(_urlHelper.Object, ProviderId, selectedCohortStatus, hasRelationship, providerAgreementStatus);
            }

            private static void PopulateWith(IEnumerable<CohortSummary> list, bool draft, Party withParty)
            {
                foreach (var item in list)
                {
                    item.IsDraft = draft;
                    item.WithParty = withParty;
                }
            }

            private CohortSummary[] CreateGetCohortsResponse()
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

                return cohorts.ToArray();
            }
        }
    }
}
