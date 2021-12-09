using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
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

            Assert.AreEqual(CohortStatus.Review, status);
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

            Assert.AreEqual(CohortStatus.Draft, status);
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

            Assert.AreEqual(CohortStatus.WithEmployer, status);
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

            Assert.AreEqual(CohortStatus.Unknown, status);
        }

        [Test]
        public void TheCohortsInDraftIsPopulatedCorrectly()
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel();

            f.VerifyCohortsInDraftIsCorrect(result);
        }

        [Test]
        public void TheCohortsInReviewIsPopulatedCorrectly()
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel();

            f.VerifyCohortsInReviewIsCorrect(result);
        }

        [Test]
        public void TheCohortsWithEmployerIsPopulatedCorrectly()
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel();

            f.VerifyCohortsWithEmployerIsCorrect(result);
        }

        [Test]
        public void TheCohortsWithTransferSenderIsPopulatedCorrectly()
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel();

            f.VerifyCohortsWithTransferSenderIsCorrect(result);
        }

        [TestCase(CohortStatus.Draft)]
        [TestCase(CohortStatus.Review)]
        [TestCase(CohortStatus.WithEmployer)]
        [TestCase(CohortStatus.WithTransferSender)]
        public void TheCohortsStatusIsSetCorrectly(CohortStatus selectedCohortStatus)
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel(selectedCohortStatus);

            f.VerifySelectedCohortStatusIsCorrect(result, selectedCohortStatus);
        }

        [TestCase(ProviderAgreementStatus.Agreed, true)]
        [TestCase(ProviderAgreementStatus.NotAgreed, false)]
        public void TheIsAgreementSignedIsPopulatedCorrectly(ProviderAgreementStatus providerAgreementStatus, bool expectedIsAgreementSigned)
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel(CohortStatus.Unknown, true, providerAgreementStatus);

            f.VerifyIsSignedAgreementIsCorrect(result, expectedIsAgreementSigned);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TheShowDraftsIsPopulatedCorrectly(bool hasRelationship, bool expectedShowDrafts)
        {
            var f = new GetCohortCardLinkViewModelTestsFixture();
            var result = f.GetCohortCardLinkViewModel(CohortStatus.Unknown, hasRelationship);

            f.VerifyShowDraftsIsCorrect(result, expectedShowDrafts);
        }

        public class GetCohortCardLinkViewModelTestsFixture
        {
            private Fixture _fixture;
            public Mock<IUrlHelper> UrlHelper { get; }

            public CohortSummary[] CohortSummaries { get; set; }

            private long providerId => 10005654;

            public GetCohortCardLinkViewModelTestsFixture()
            {
                UrlHelper = new Mock<IUrlHelper>();
                UrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");
                _fixture = new Fixture();

                CohortSummaries = CreateGetCohortsResponse();
            }

            public void VerifyCohortsInDraftIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.IsNotNull(result.CohortsInDraft);
                Assert.AreEqual(5, result.CohortsInDraft.Count);
                Assert.AreEqual("Drafts", result.CohortsInDraft.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Draft")));
            }

            public void VerifyCohortsInReviewIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.IsNotNull(result.CohortsInReview);
                Assert.AreEqual(4, result.CohortsInReview.Count);
                Assert.AreEqual("Ready to review", result.CohortsInReview.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "Review")));
            }

            public void VerifyCohortsWithEmployerIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.IsNotNull(result.CohortsWithEmployer);
                Assert.AreEqual(3, result.CohortsWithEmployer.Count);
                Assert.AreEqual("With employers", result.CohortsWithEmployer.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithEmployer")));
            }

            public void VerifyCohortsWithTransferSenderIsCorrect(ApprenticeshipRequestsHeaderViewModel result)
            {
                Assert.IsNotNull(result.CohortsWithTransferSender);
                Assert.AreEqual(2, result.CohortsWithTransferSender.Count);
                Assert.AreEqual("With transfer sending employers", result.CohortsWithTransferSender.Description);
                UrlHelper.Verify(x => x.Action(It.Is<UrlActionContext>(p => p.Controller == "Cohort" && p.Action == "WithTransferSender")));
            }

            public void VerifySelectedCohortStatusIsCorrect(ApprenticeshipRequestsHeaderViewModel result, CohortStatus expectedCohortStatus)
            {
                Assert.AreEqual(expectedCohortStatus == CohortStatus.WithTransferSender, result.CohortsWithTransferSender.IsSelected);
                Assert.AreEqual(expectedCohortStatus == CohortStatus.Draft, result.CohortsInDraft.IsSelected);
                Assert.AreEqual(expectedCohortStatus == CohortStatus.Review, result.CohortsInReview.IsSelected);
                Assert.AreEqual(expectedCohortStatus == CohortStatus.WithEmployer, result.CohortsWithEmployer.IsSelected);
            }

            public void VerifyIsSignedAgreementIsCorrect(ApprenticeshipRequestsHeaderViewModel result, bool expectedIsAgreementSigned)
            {
                Assert.AreEqual(expectedIsAgreementSigned, result.IsAgreementSigned);
            }

            public void VerifyShowDraftsIsCorrect(ApprenticeshipRequestsHeaderViewModel result, bool expectedShowDrafts)
            {
                Assert.AreEqual(expectedShowDrafts, result.ShowDrafts);
            }

            public ApprenticeshipRequestsHeaderViewModel GetCohortCardLinkViewModel(CohortStatus selectedCohortStatus = CohortStatus.Draft, bool hasRelationship = true, ProviderAgreementStatus providerAgreementStatus = ProviderAgreementStatus.Agreed)
            {
                return CohortSummaries.GetCohortCardLinkViewModel(UrlHelper.Object, providerId, selectedCohortStatus, hasRelationship, providerAgreementStatus);
            }

            private static List<CohortSummary> PopulateWith(List<CohortSummary> list, bool draft, Party withParty)
            {
                foreach (var item in list)
                {
                    item.IsDraft = draft;
                    item.WithParty = withParty;
                }

                return list;
            }

            private CohortSummary[] CreateGetCohortsResponse()
            {
                var listInDraft = _fixture.CreateMany<CohortSummary>(5).ToList();
                PopulateWith(listInDraft, true, Party.Provider);

                var listInReview = _fixture.CreateMany<CohortSummary>(4).ToList();
                PopulateWith(listInReview, false, Party.Provider);

                var listWithEmployer = _fixture.CreateMany<CohortSummary>(3).ToList();
                PopulateWith(listWithEmployer, false, Party.Employer).ToList();

                var listWithTransferSender = _fixture.CreateMany<CohortSummary>(2).ToList();
                PopulateWith(listWithTransferSender, false, Party.TransferSender);

                var cohorts = listInDraft.Union(listInReview).Union(listWithEmployer).Union(listWithTransferSender);

                return cohorts.ToArray();
            }
        }
    }
}
