using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WhenMappingAcknowledgementRequestToViewModel
    {
        [Test]
        public async Task ProviderIdIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task WithPartyIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.WithParty, result.WithParty);
        }

        [Test]
        public async Task EmployerNameIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.LegalEntityName, result.EmployerName);
        }

        [Test]
        public async Task ProviderNameIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.ProviderName, result.ProviderName);
        }

        [Test]
        public async Task MessageIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.LatestMessageCreatedByProvider, result.Message);
        }

        [Test]
        public async Task WhenNoMessageIsSpecifiedMessageIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            fixture.Cohort.LatestMessageCreatedByProvider = string.Empty;

            var result = await fixture.Map();
            Assert.AreEqual("No message added", result.Message);
        }

        [Test]
        public async Task CohortReferenceIsMappedCorrectly()
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Source.CohortReference, result.CohortReference);
        }

        public enum ExpectedWhatHappensNextType
        {
            TransferFirstApproval,
            EmployerWillReview,
            UpdatedCohort
        }

        [TestCase(true, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.TransferFirstApproval)]
        [TestCase(true, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        [TestCase(false, SaveStatus.ApproveAndSend, ExpectedWhatHappensNextType.EmployerWillReview)]
        [TestCase(false, SaveStatus.Approve, ExpectedWhatHappensNextType.UpdatedCohort)]
        public async Task ThenWhatHappensNextIsPopulatedCorrectly(bool isTransfer, SaveStatus saveStatus, ExpectedWhatHappensNextType expectedWhatHappensNextType)
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();

            fixture.Cohort.TransferSenderId = isTransfer ? 100 : default(long?);
            fixture.Cohort.ChangeOfPartyRequestId = isTransfer ? default(long?) : 100;

            fixture.CommitmentsApiClient
                .Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fixture.Cohort);

            fixture.Source.SaveStatus = saveStatus;

            var result = await fixture.Map();

            string[] expectedWhatHappensNext;
            switch (expectedWhatHappensNextType)
            {
                case ExpectedWhatHappensNextType.TransferFirstApproval:
                    expectedWhatHappensNext = new[]
                    {
                        "The employer will review the cohort and either approve or contact you with an update.",
                        "Once the employer approves the cohort, a transfer request will be sent to the funding employer to review.",
                        "You’ll receive a notification when the funding employer approves or rejects the transfer request."
                    };
                    break;
                case ExpectedWhatHappensNextType.EmployerWillReview:
                    expectedWhatHappensNext = new[] { "The employer will review the cohort and either approve it or contact you with an update." };
                    break;
                case ExpectedWhatHappensNextType.UpdatedCohort:
                    expectedWhatHappensNext = new[] { "The updated cohort will appear in the employer’s account for them to review." };
                    break;
                default:
                    throw new NotImplementedException();
            }

            CollectionAssert.AreEquivalent(expectedWhatHappensNext, result.WhatHappensNext);
        }


        [TestCase(true, true,  SaveStatus.Approve, "Cohort approved")]
        [TestCase(true, false, SaveStatus.ApproveAndSend, "Cohort approved and sent to employer")]
        [TestCase(false, false, SaveStatus.AmendAndSend, "Cohort sent to employer for review")]
        public async Task ThenPageTitleMappedCorrectly(bool isApprovedByProvider, bool isApprovedByEmployer, SaveStatus saveStatus, string expectedText)
        {
            var fixture = new WhenMappingAcknowledgementRequestToViewModelTestsFixture();

            fixture.Cohort.IsApprovedByProvider = isApprovedByProvider;
            fixture.Cohort.IsApprovedByEmployer = isApprovedByEmployer;

            fixture.CommitmentsApiClient
                .Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fixture.Cohort);

            fixture.Source.SaveStatus = saveStatus;

            var result = await fixture.Map();

            Assert.AreEqual(expectedText, result.PageTitle);
        }

    }

    public class WhenMappingAcknowledgementRequestToViewModelTestsFixture
    {
        public AcknowledgementRequestViewModelMapper Mapper;
        public AcknowledgementRequest Source;
        public AcknowledgementViewModel Result;
        public Mock<ICommitmentsApiClient> CommitmentsApiClient;
        public GetCohortResponse Cohort;
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse;
        private Fixture _autoFixture;

        public WhenMappingAcknowledgementRequestToViewModelTestsFixture()
        {
            _autoFixture = new Fixture();

            Cohort = _autoFixture.Create<GetCohortResponse>();

            DraftApprenticeshipsResponse = _autoFixture.Create<GetDraftApprenticeshipsResponse>();

            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Cohort);

            CommitmentsApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DraftApprenticeshipsResponse);

            Mapper = new AcknowledgementRequestViewModelMapper(CommitmentsApiClient.Object);
            Source = _autoFixture.Create<AcknowledgementRequest>();
        }

        public WhenMappingAcknowledgementRequestToViewModelTestsFixture SetCohortWithParty(Party party)
        {
            Cohort.WithParty = party;
            return this;
        }

        public Task<AcknowledgementViewModel> Map()
        {
            return Mapper.Map(TestHelper.Clone(Source));
        }
    }
}
