using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class DetailsViewModelMapperTests
    {
        private DetailsViewModelMapperFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DetailsViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.Source.ApprenticeshipHashedId, _fixture.Result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenFullNameIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.FirstName + " " + _fixture.ApiResponse.Apprenticeship.LastName, _fixture.Result.ApprenticeName);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.EmployerName, _fixture.Result.Employer);
        }

        [Test]
        public async Task ThenReferenceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.CohortReference, _fixture.Result.Reference);
        }

        [Test]
        public async Task ThenStatusIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.Status, _fixture.Result.Status);
        }

        [Test]
        public async Task ThenStopDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.StopDate, _fixture.Result.StopDate);
        }

        [Test]
        public async Task ThenCanResendInvitationIsFalse()
        {
            _fixture.ApiResponse.Apprenticeship.Status = ApprenticeshipStatus.Stopped;
            await _fixture.Map();
            Assert.IsFalse(_fixture.Result.CanResendInvitation);
        }

        [Test]
        public async Task ThenPauseDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.PauseDate, _fixture.Result.PauseDate);
        }

        [Test]
        public async Task ThenCompletionDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.CompletionDate, _fixture.Result.CompletionDate);
        }

        [Test]
        public async Task ThenAgreementIdIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.AgreementId, _fixture.Result.AgreementId);
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.DateOfBirth, _fixture.Result.DateOfBirth);
        }

        [Test]
        public async Task ThenUlnIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.Uln, _fixture.Result.Uln);
        }

        [Test]
        public async Task ThenCourseNameIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.CourseName, _fixture.Result.CourseName);
        }

        [Test]
        public async Task ThenOptionIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.Option, _fixture.Result.Option);
        }

        [Test]
        public async Task ThenVersionIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.Version, _fixture.Result.Version);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.StartDate, _fixture.Result.StartDate);
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.ActualStartDate, _fixture.Result.ActualStartDate);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.EmploymentEndDate, _fixture.Result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.EndDate, _fixture.Result.EndDate);
        }

        [Test]
        public async Task ThenProviderRefIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.ProviderReference, _fixture.Result.ProviderRef);
        }

        [Test]
        public async Task ThenRplDataIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.RecognisePriorLearning, _fixture.Result.RecognisePriorLearning);
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.TrainingTotalHours, _fixture.Result.TrainingTotalHours);
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.DurationReducedByHours, _fixture.Result.DurationReducedByHours);
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.DurationReducedBy, _fixture.Result.DurationReducedBy);
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.PriceReducedBy, _fixture.Result.PriceReducedBy);
        }


        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.EmploymentPrice, _fixture.Result.EmploymentPrice);
        }

        [Test]
        public async Task ThenPriceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.PriceEpisodes.GetCost(), _fixture.Result.Cost);
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.PriceEpisodes.First().TrainingPrice, _fixture.Result.TrainingPrice);
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.PriceEpisodes.First().EndPointAssessmentPrice, _fixture.Result.EndPointAssessmentPrice);
        }

        [Test]
        public async Task ThenDeliveryModelIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.DeliveryModel, _fixture.Result.DeliveryModel);
        }

        [Test]
        public async Task ThenRecognisePriorLearningIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.RecognisePriorLearning, _fixture.Result.RecognisePriorLearning);
        }

        [Test]
        public async Task ThenDurationReducedByIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.DurationReducedBy, _fixture.Result.DurationReducedBy);
        }

        [Test]
        public async Task ThenPriceReducedByIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Apprenticeship.PriceReducedBy, _fixture.Result.PriceReducedBy);
        }

        [TestCase(ApprenticeshipStatus.Live, true)]
        [TestCase(ApprenticeshipStatus.Paused, true)]
        [TestCase(ApprenticeshipStatus.WaitingToStart, true)]
        [TestCase(ApprenticeshipStatus.Stopped, false)]
        [TestCase(ApprenticeshipStatus.Completed, false)]
        public async Task ThenAllowEditApprenticeIsMappedCorrectly(ApprenticeshipStatus status, bool expectedAllowEditApprentice)
        {
            _fixture
                .WithApprenticeshipFlexiPilotStatus(false)
                .WithApprenticeshipStatus(status);

            await _fixture.Map();

            Assert.AreEqual(expectedAllowEditApprentice, _fixture.Result.AllowEditApprentice);
        }

        [TestCase(null)]
        [TestCase(ConfirmationStatus.Unconfirmed)]
        [TestCase(ConfirmationStatus.Confirmed)]
        [TestCase(ConfirmationStatus.Overdue)]
        public async Task ThenConfirmationStatusIsMappedCorrectly(ConfirmationStatus? status)
        {
            _fixture.WithApprenticeshipConfirmationStatus(status);

            await _fixture.Map();

            Assert.AreEqual(status, _fixture.Result.ConfirmationStatus);
        }

        [TestCase]
        public async Task WhenPendingUpdates_ThenAllowEditApprenticeIsMappedCorrectly()
        {
            _fixture.WithPendingUpdatesForProvider();

            await _fixture.Map();

            Assert.AreEqual(false, _fixture.Result.AllowEditApprentice);
        }

        [Test]
        public async Task WhenThereAreNoDataLocks_ThenAllowEditApprenticeIsTrue()
        {
            _fixture.WithApprenticeshipFlexiPilotStatus(false);

            await _fixture.Map();

            Assert.IsTrue(_fixture.Result.AllowEditApprentice);
        }

        [TestCase(TriageStatus.Change)]
        [TestCase(TriageStatus.Restart)]
        [TestCase(TriageStatus.FixIlr)]
        public async Task WhenThereAreDataLocksInTriage_ThenAllowEditApprenticeIsFalse(TriageStatus triageStatus)
        {
            _fixture.WithUnResolvedDataLocksInTriage(triageStatus);

            await _fixture.Map();

            Assert.IsFalse(_fixture.Result.AllowEditApprentice);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(null, true)]
        public async Task ThenAllowEditApprenticeIsMappedCorrectly(bool? pilotStatus, bool expectedAllowEditApprentice)
        {
            _fixture.WithApprenticeshipFlexiPilotStatus(pilotStatus);

            await _fixture.Map();

            Assert.AreEqual(expectedAllowEditApprentice, _fixture.Result.AllowEditApprentice);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task When_ApprenticeshipIsStandard_Then_HasOptionIsMappedCorrectly(bool hasOptions)
        {
            if (hasOptions)
                _fixture.WithOptions();

            await _fixture.Map();

            Assert.AreEqual(hasOptions, _fixture.Result.HasOptions);
        }

        [Test]
        public async Task When_ApprenticeshipIsFramework_Then_HasOptionsIsFalse()
        {
            _fixture.WithFramework();

            await _fixture.Map();

            Assert.False(_fixture.Result.HasOptions);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenProviderPendingUpdateIsMappedCorrectly(bool pendingUpdate)
        {
            if (pendingUpdate)
            {
                _fixture.WithPendingUpdatesForProvider();
            }

            await _fixture.Map();

            Assert.AreEqual(pendingUpdate, _fixture.Result.HasProviderPendingUpdate);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenEmployerPendingUpdateIsMappedCorrectly(bool pendingUpdate)
        {
            if (pendingUpdate)
            {
                _fixture.WithPendingUpdatesForEmployer();
            }

            await _fixture.Map();

            Assert.AreEqual(pendingUpdate, _fixture.Result.HasEmployerPendingUpdate);
        }

        [Test]
        public async Task When_ProviderPendingUpdates_HasEmployerPendingUpdate_IsFalse()
        {
            _fixture.WithPendingUpdatesForProvider();
            await _fixture.Map();
            Assert.AreEqual(false, _fixture.Result.HasEmployerPendingUpdate);
        }

        [Test]
        public async Task When_EmployerPendingUpdates_HasProviderPendingUpdate_IsFalse()
        {
            _fixture.WithPendingUpdatesForProvider();
            await _fixture.Map();
            Assert.AreEqual(false, _fixture.Result.HasEmployerPendingUpdate);
        }

        [Test]
        public async Task When_DataLocks_AreUnresolvedAndFailed_Then_DataLockStatus_IsHasUnresolvedDataLocks()
        {
            _fixture.WithUnresolvedAndFailedDataLocks();
            await _fixture.Map();
            Assert.AreEqual(DetailsViewModel.DataLockSummaryStatus.HasUnresolvedDataLocks, 
                _fixture.Result.DataLockStatus);
        }

        [Test]
        public async Task When_DataLocks_AreUnresolvedAndPassing_Then_DataLockStatus_IsNone()
        {
            _fixture.WithUnResolvedAndPassingDataLocks();
            await _fixture.Map();
            Assert.AreEqual(DetailsViewModel.DataLockSummaryStatus.None,
                _fixture.Result.DataLockStatus);
        }

        [Test]
        public async Task When_DataLocks_AreResolved_Then_DataLockStatus_IsNone()
        {
            _fixture.WithResolvedDataLocks();
            await _fixture.Map();
            Assert.AreEqual(DetailsViewModel.DataLockSummaryStatus.None,
                _fixture.Result.DataLockStatus);
        }

        [TestCase(TriageStatus.Change)]
        [TestCase(TriageStatus.Restart)]
        [TestCase(TriageStatus.FixIlr)]
        public async Task When_DataLocks_AreUnresolvedButInTriage_Then_DataLockStatus_IsAwaitingTriage(TriageStatus triageStatus)
        {
            _fixture.WithUnResolvedDataLocksInTriage(triageStatus);
            await _fixture.Map();
            Assert.AreEqual(DetailsViewModel.DataLockSummaryStatus.AwaitingTriage,
                _fixture.Result.DataLockStatus);
        }

        [Test]
        public async Task When_PendingEmployerUpdates_Then_SuppressDataLockStatusLink_IsTrue()
        {
            _fixture.WithPendingUpdatesForEmployer();
            await _fixture.Map();
            Assert.IsTrue(_fixture.Result.SuppressDataLockStatusReviewLink);
        }

        [Test]
        public async Task When_PendingProviderUpdates_Then_SuppressDataLockStatusLink_IsTrue()
        {
            _fixture.WithPendingUpdatesForProvider();
            await _fixture.Map();
            Assert.IsTrue(_fixture.Result.SuppressDataLockStatusReviewLink);
        }

        [TestCase(false, DataLockErrorCode.Dlock04, DetailsViewModel.TriageOption.Update)]
        [TestCase(false, DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Update)]
        [TestCase(false, DataLockErrorCode.Dlock07 | DataLockErrorCode.Dlock04, DetailsViewModel.TriageOption.Update)]
        [TestCase(true, DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Update)]
        [TestCase(true, DataLockErrorCode.Dlock04, DetailsViewModel.TriageOption.Restart)]
        [TestCase(true, DataLockErrorCode.Dlock04 | DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Restart)]
        public async Task With_Single_Datalock_Then_AvailableTriageOption_Is_Mapped_Correctly(bool hasHadDataLockSuccess, DataLockErrorCode dataLockErrorCode, DetailsViewModel.TriageOption expectedTriageOption)
        {
            _fixture
                .WithHasHadDataLockSuccess(hasHadDataLockSuccess)
                .WithUnresolvedAndFailedDataLocks(dataLockErrorCode);

            await _fixture.Map();

            Assert.AreEqual(expectedTriageOption, _fixture.Result.AvailableTriageOption);
        }


        [TestCase(false, DataLockErrorCode.Dlock04, DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Update)]
        [TestCase(true, DataLockErrorCode.Dlock04, DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Both)]
        [TestCase(true, DataLockErrorCode.Dlock03, DataLockErrorCode.Dlock03 | DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Restart)]
        [TestCase(true, DataLockErrorCode.Dlock07, DataLockErrorCode.Dlock03 | DataLockErrorCode.Dlock07, DetailsViewModel.TriageOption.Restart)]
        public async Task With_Multiple_Datalocks_Then_AvailableTriageOption_Is_Mapped_Correctly(bool hasHadDataLockSuccess, DataLockErrorCode dataLockErrorCode, DataLockErrorCode dataLock2ErrorCode, DetailsViewModel.TriageOption expectedTriageOption)
        {
            _fixture
                .WithHasHadDataLockSuccess(hasHadDataLockSuccess)
                .WithUnresolvedAndFailedDataLocks(dataLockErrorCode)
                .WithAnotherDataLock(dataLock2ErrorCode);

            await _fixture.Map();

            Assert.AreEqual(expectedTriageOption, _fixture.Result.AvailableTriageOption);
        }

        [TestCase(null, false)]
        [TestCase(ChangeOfPartyRequestStatus.Approved, false)]
        [TestCase(ChangeOfPartyRequestStatus.Rejected, false)]
        [TestCase(ChangeOfPartyRequestStatus.Withdrawn, false)]
        [TestCase(ChangeOfPartyRequestStatus.Pending, true)]
        public async Task ThenHasChangeOfPartyRequestPendingIsMappedCorrectly(ChangeOfPartyRequestStatus? status, bool expectHasPending)
        {
            if (status.HasValue)
            {
                _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, status.Value);
            }

            await _fixture.Map();

            Assert.AreEqual(expectHasPending, _fixture.Result.HasPendingChangeOfPartyRequest);
        }

        [TestCase(Party.Employer)]
        [TestCase(Party.Provider)]
        public async Task ThenPendingChangeOfPartyRequestWithPartyIsMappedCorrectly(Party withParty)
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Pending, withParty);
            await _fixture.Map();
            Assert.AreEqual(withParty, _fixture.Result.PendingChangeOfPartyRequestWithParty);
        }

        [Test]
        public async Task ThenIfNextApprenticeshipThenHasContinuationIsMappedCorrectly()
        {
            _fixture.WithNextApprenticeship();
            await _fixture.Map();
            Assert.IsTrue(_fixture.Result.HasContinuation);
        }

        [Test]
        public async Task ThenIfNoNextApprenticeshipThenHasContinuationIsMappedCorrectly()
        {
            _fixture.WithoutNextApprenticeship();
            await _fixture.Map();
            Assert.IsFalse(_fixture.Result.HasContinuation);
        }

        [Test]
        public async Task ThenAPendingChangeOfPartyOriginatingFromEmployerDoesNotSetHasPendingChangeOfPartyRequest()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeProvider, ChangeOfPartyRequestStatus.Pending);

            await _fixture.Map();

            Assert.IsFalse(_fixture.Result.HasPendingChangeOfPartyRequest);
        }

        public async Task WhenNoNextApprenticeshipThenShowChangeEmployerLinkIsMappedCorrectly()
        {
            //Arrange 
            _fixture
                .WithApprenticeshipStatus(ApprenticeshipStatus.Stopped)
                .WithoutNextApprenticeship();

            //Act
            await _fixture.Map();

            //Assert
            Assert.AreEqual(true, _fixture.Result.ShowChangeEmployerLink);
        }

        public async Task WhenNextApprenticeshipThenShowChangeEmployerLinkIsMappedCorrectly()
        {
            //Arrange 
            _fixture
                .WithApprenticeshipStatus(ApprenticeshipStatus.Stopped)
                .WithNextApprenticeship();

            //Act
            await _fixture.Map();

            //Assert
            Assert.AreEqual(false, _fixture.Result.ShowChangeEmployerLink);
        }

        [TestCase(ApprenticeshipStatus.Stopped, true)]
        [TestCase(ApprenticeshipStatus.Paused, false)]
        [TestCase(ApprenticeshipStatus.Live, false)]
        [TestCase(ApprenticeshipStatus.WaitingToStart, false)]
        [TestCase(ApprenticeshipStatus.Completed, false)]
        public async Task WhenApprenticeStatusThenShowChangeEmployerLinkIsMappedCorrectly(ApprenticeshipStatus apprenticeshipStatus, bool expected)
        {
            //Arrange
            _fixture
                .WithApprenticeshipStatus(apprenticeshipStatus)
                .WithoutNextApprenticeship();

            //Act
            await _fixture.Map();

            //Assert
            Assert.AreEqual(expected, _fixture.Result.ShowChangeEmployerLink);
        }

        [Test]
        public async Task ThenIfChangeOfEmployerChainThenEmployerHistoryIsMappedCorrectly()
        {
            _fixture.WithChangeOfEmployerChain();
            await _fixture.Map();
            Assert.IsNotNull(_fixture.Result.EmployerHistory);
        }

        [Test]
        public async Task CheckEmailIsMappedCorrectly()
        {
            var email = "a@a.com";
            _fixture.WithEmailPopulated(email);

            var result = await _fixture.Map();

            Assert.AreEqual(email, _fixture.Result.Email);
        }

        [TestCase(false, Party.None, false, false)]
        [TestCase(true, Party.None, false, true)]
        [TestCase(false, Party.Employer, false, false)]
        [TestCase(false, Party.Provider, false, true)]
        [TestCase(false, Party.None, true, true)]
        [TestCase(true, Party.Employer, false, true)]
        [TestCase(true, Party.Provider, false, true)]
        [TestCase(true, Party.None, true, true)]
        [TestCase(false, Party.Employer, true, true)]
        [TestCase(false, Party.Provider, true, true)]
        public async Task CheckActionRequiredBannerIsShownCorrectly(bool unresolvedDataLocks, Party? party, bool employerUpdate, bool expected)
        {
            if(unresolvedDataLocks) _fixture.WithUnresolvedAndFailedDataLocks();
            if (employerUpdate) _fixture.WithPendingUpdatesForEmployer();
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Pending, party);

            await _fixture.Map();

            Assert.AreEqual(expected, _fixture.Result.ShowActionRequiredBanner);
        }

        [TestCase(false, Party.None, false, false)]
        [TestCase(true, Party.None, false, true)]
        [TestCase(false, Party.Employer, false, true)]
        [TestCase(false, Party.Provider, false, false)]
        [TestCase(false, Party.None, true, true)]
        [TestCase(true, Party.Employer, false, true)]
        [TestCase(true, Party.Provider, false, true)]
        [TestCase(true, Party.None, true, true)]
        [TestCase(false, Party.Employer, true, true)]
        [TestCase(false, Party.Provider, true, true)]
        public async Task CheckChangesToThisApprenticeshipBannerIsShownCorrectly(bool unresolvedDataLocks, Party? party, bool providerUpdate, bool expected)
        {
            if (unresolvedDataLocks) _fixture.WithUnResolvedDataLocksInTriage(TriageStatus.Change);
            if (providerUpdate) _fixture.WithPendingUpdatesForProvider();
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer,
                ChangeOfPartyRequestStatus.Pending, party);

            await _fixture.Map();

            Assert.AreEqual(expected, _fixture.Result.ShowChangesToThisApprenticeshipBanner);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task CheckEmailShouldBePresentIsMappedCorrectly(bool expected)
        {
            _fixture.WithEmailShouldBePresentPopulated(expected);

            var result = await _fixture.Map();

            Assert.AreEqual(expected, _fixture.Result.EmailShouldBePresent);
        }

        [Test]
        public async Task And_ApprenticeshipIsAFramework_Then_ShowChangeVersionLinkIsFalse()
        {
            _fixture.WithFramework();

            await _fixture.Map();

            Assert.AreEqual(false, _fixture.Result.ShowChangeVersionLink);
        }

        [Test]
        public async Task And_NewerVersionExists_Then_ShowChangeVersionLinkIsTrue()
        {
            _fixture.WithNewerVersions();

            await _fixture.Map();

            Assert.AreEqual(true, _fixture.Result.ShowChangeVersionLink);
        }

        [Test]
        public async Task And_NoNewerVersionExists_Then_ShowChangeVersionLinkIsFalse()
        {
            //_fixture.WithoutNewerVersions();

            await _fixture.Map();

            Assert.AreEqual(false, _fixture.Result.ShowChangeVersionLink);
        }

        [Test]
        public async Task CheckIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var isOnPilot = true;
            _fixture.WithIsOnFlexiPaymentPilotPopulated(isOnPilot);

            var result = await _fixture.Map();

            Assert.AreEqual(isOnPilot, _fixture.Result.IsOnFlexiPaymentPilot);
        }

        [Test]
        public async Task And_PriceChangeDetailsIsNull_Then_PriceChangeDetailsNotReturned()
        {
            _fixture.WithoutPendingPriceChangePopulated();

            await _fixture.Map();

            Assert.IsNull(_fixture.Result.PendingPriceChange);
        }

        [Test]
        public async Task And_PriceChangeDetailsArePopulated_Then_PriceChangeDetailsReturned()
        {
            _fixture.WithPendingPriceChangePopulated();

            await _fixture.Map();

            Assert.IsNotNull(_fixture.Result.PendingPriceChange);
            Assert.AreEqual(_fixture.ApiResponse.PendingPriceChange.Cost, _fixture.Result.PendingPriceChange.Cost);
            Assert.AreEqual(_fixture.ApiResponse.PendingPriceChange.EndPointAssessmentPrice, _fixture.Result.PendingPriceChange.EndPointAssessmentPrice);
            Assert.AreEqual(_fixture.ApiResponse.PendingPriceChange.TrainingPrice, _fixture.Result.PendingPriceChange.TrainingPrice);
        }

        public class DetailsViewModelMapperFixture
        {
            private DetailsViewModelMapper _sut;
            public DetailsRequest Source { get; }
            public DetailsViewModel Result { get; private set; }
            public GetManageApprenticeshipDetailsResponse ApiResponse { get; }
            public GetApprenticeshipResponse ApiResponseOld { get; }
            public GetManageApprenticeshipDetailsResponse.ApprenticeshipDetails ApprenticeshipDetails { get; }
            public IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> PriceEpisodes { get; }
            public IEnumerable<GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate> ApprenticeshipUpdates { get; private set; }
            public IEnumerable<GetManageApprenticeshipDetailsResponse.DataLock> DataLocks { get; private set; }
            public IEnumerable<GetManageApprenticeshipDetailsResponse.ChangeOfPartyRequest> ChangeOfPartyRequests { get; private set; }
            public IEnumerable<GetManageApprenticeshipDetailsResponse.ChangeOfEmployerLink> ChangeOfEmployerChain { get; private set; }
            public GetNewerTrainingProgrammeVersionsResponse GetNewerTrainingProgrammeVersionsResponse { get; private set; }
            public GetTrainingProgrammeResponse GetTrainingProgrammeByStandardUIdResponse { get; private set; }

            private readonly Mock<IEncodingService> _encodingService;            
            private readonly Mock<IAuthorizationService> _authorizationService;            
            public string CohortReference { get; }
            public string AgreementId { get; }
            public string URL { get; }
            public Fixture Fixture { get; }
            public string EncodedNewApprenticeshipId { get; }
            public string EncodedPreviousApprenticeshipId { get; }
            public string EncodedNextApprenticeshipId { get; }

            public DetailsViewModelMapperFixture()
            {
                Fixture = new Fixture();
                Source = Fixture.Create<DetailsRequest>();
                ApiResponse = Fixture.Create<GetManageApprenticeshipDetailsResponse>();
                ApiResponse.Apprenticeship.ProviderId = Source.ProviderId;
                CohortReference = Fixture.Create<string>();
                AgreementId = Fixture.Create<string>();
                URL = Fixture.Create<string>();
                ApiResponse.PriceEpisodes = new List<GetManageApprenticeshipDetailsResponse.PriceEpisode>
                {
                    new GetManageApprenticeshipDetailsResponse.PriceEpisode {Cost = 100, FromDate = DateTime.UtcNow}
                };

                ApprenticeshipUpdates = new List<GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate>();
                DataLocks = new List<GetManageApprenticeshipDetailsResponse.DataLock>();
                ChangeOfPartyRequests = new List<GetManageApprenticeshipDetailsResponse.ChangeOfPartyRequest>();
                ChangeOfEmployerChain = new List<GetManageApprenticeshipDetailsResponse.ChangeOfEmployerLink>();

                ApiResponse.ApprenticeshipUpdates = ApprenticeshipUpdates;
                ApiResponse.DataLocks = DataLocks;
                ApiResponse.ChangeOfPartyRequests = ChangeOfPartyRequests;
                ApiResponse.ChangeOfEmployerChain = ChangeOfEmployerChain;

                GetNewerTrainingProgrammeVersionsResponse = new GetNewerTrainingProgrammeVersionsResponse()
                {
                    NewerVersions = new List<TrainingProgramme>()
                };

                GetTrainingProgrammeByStandardUIdResponse = new GetTrainingProgrammeResponse();

                _encodingService = new Mock<IEncodingService>();
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns(CohortReference);
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns(AgreementId);

                _authorizationService = new Mock<IAuthorizationService>();

                EncodedNewApprenticeshipId = Fixture.Create<string>();
                EncodedPreviousApprenticeshipId = Fixture.Create<string>();
            }

            public async Task<DetailsViewModelMapperFixture> Map()
            {
                var apiClient = new Mock<ICommitmentsApiClient>();
                var commitmentsApiClient = new Mock<IOuterApiClient>();

                commitmentsApiClient.Setup(x =>
                        x.Get<GetManageApprenticeshipDetailsResponse>(It.IsAny<GetManageApprenticeshipDetailsRequest>()))
                    .ReturnsAsync(ApiResponse);

                apiClient.Setup(x => x.GetNewerTrainingProgrammeVersions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetNewerTrainingProgrammeVersionsResponse);

                apiClient.Setup(x => x.GetTrainingProgrammeVersionByStandardUId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetTrainingProgrammeByStandardUIdResponse);

                _sut = new DetailsViewModelMapper(apiClient.Object, _encodingService.Object, commitmentsApiClient.Object, Mock.Of<ILogger<DetailsViewModelMapper>>());

                Result = await _sut.Map(Source);
                return this;
            }

            public DetailsViewModelMapperFixture WithFramework()
            {
                ApiResponse.Apprenticeship.CourseCode = "123-1-2";
                return this;
            }

            public DetailsViewModelMapperFixture WithNewerVersions()
            {
                ApiResponse.Apprenticeship.StandardUId = "ST0001_1.0";

                var newerTrainingProgramme = Fixture.Build<TrainingProgramme>()
                    .With(x => x.CourseCode, "1")
                    .With(x => x.StandardUId, "ST0001_1.1").Create();

                GetNewerTrainingProgrammeVersionsResponse.NewerVersions = new List<TrainingProgramme> { newerTrainingProgramme };

                return this;
            }

            public DetailsViewModelMapperFixture WithOptions()
            {
                ApiResponse.Apprenticeship.StandardUId = "ST0001_1.0";

                var trainingProgramme = Fixture.Build<TrainingProgramme>()
                    .With(x => x.Options, Fixture.Create<List<string>>())
                    .Create();

                GetTrainingProgrammeByStandardUIdResponse.TrainingProgramme = trainingProgramme;

                return this;
            }

            public DetailsViewModelMapperFixture WithApprenticeshipStatus(
                ApprenticeshipStatus status)
            {
                ApiResponse.Apprenticeship.Status = status;
                return this;
            }

            public DetailsViewModelMapperFixture WithApprenticeshipDeliveryModel(DeliveryModel dm)
            {
                ApiResponse.Apprenticeship.DeliveryModel = dm;
                return this;
            }

            public DetailsViewModelMapperFixture WithApprenticeshipConfirmationStatus(
                ConfirmationStatus? status)
            {
                ApiResponse.Apprenticeship.ConfirmationStatus = status;
                return this;
            }

            public DetailsViewModelMapperFixture WithApprenticeshipFlexiPilotStatus(
                bool? pilotStatus)
            {
                ApiResponse.Apprenticeship.IsOnFlexiPaymentPilot = pilotStatus;
                return this;
            }

            public DetailsViewModelMapperFixture WithPendingUpdatesForProvider()
            {
                ApiResponse.ApprenticeshipUpdates =
                    new List<GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate>()
                    {
                        new GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate
                        {
                            Id = 1,
                            OriginatingParty = Party.Provider
                        }
                    };
                return this;
            }

            public DetailsViewModelMapperFixture WithPendingUpdatesForEmployer()
            {
                ApiResponse.ApprenticeshipUpdates =
                    new List<GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate>()
                    {
                        new GetManageApprenticeshipDetailsResponse.ApprenticeshipUpdate
                        {
                            Id = 1,
                            OriginatingParty = Party.Employer
                        }
                    };
                return this;
            }

            public DetailsViewModelMapperFixture WithResolvedDataLocks()
            {
                ApiResponse.DataLocks = new List<GetManageApprenticeshipDetailsResponse.DataLock> { 
                    new GetManageApprenticeshipDetailsResponse.DataLock
                    {
                        Id = 1,
                        TriageStatus = TriageStatus.Unknown,
                        DataLockStatus = Status.Fail,
                        IsResolved = true
                    },
                    new GetManageApprenticeshipDetailsResponse.DataLock
                    {
                        Id = 2,
                        TriageStatus = TriageStatus.Unknown,
                        DataLockStatus = Status.Pass,
                        IsResolved = true
                    }
                };
                return this;
            }

            public DetailsViewModelMapperFixture WithUnresolvedAndFailedDataLocks(DataLockErrorCode errorCode = DataLockErrorCode.Dlock07)
            {
                ApiResponse.DataLocks = new List<GetManageApprenticeshipDetailsResponse.DataLock> { new GetManageApprenticeshipDetailsResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Fail,
                    IsResolved = false,
                    ErrorCode = errorCode
                }};
                return this;
            }

            public DetailsViewModelMapperFixture WithAnotherDataLock(DataLockErrorCode errorCode)
            {
                var dataLocks = ApiResponse.DataLocks.ToList();
                dataLocks.Add(new GetManageApprenticeshipDetailsResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Fail,
                    IsResolved = false,
                    ErrorCode = errorCode
                });

                ApiResponse.DataLocks = dataLocks.AsReadOnly();
                return this;
            }

            public DetailsViewModelMapperFixture WithUnResolvedAndPassingDataLocks()
            {
                ApiResponse.DataLocks = new List<GetManageApprenticeshipDetailsResponse.DataLock> { new GetManageApprenticeshipDetailsResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Pass,
                    IsResolved = false
                }};
                return this;
            }

            public DetailsViewModelMapperFixture WithUnResolvedDataLocksInTriage(TriageStatus triageStatus)
            {
                ApiResponse.DataLocks = new List<GetManageApprenticeshipDetailsResponse.DataLock> { new GetManageApprenticeshipDetailsResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = triageStatus,
                    DataLockStatus = Status.Fail,
                    IsResolved = false
                }};
                return this;
            }

            public DetailsViewModelMapperFixture WithHasHadDataLockSuccess(bool hasHadDataLockSuccess)
            {
                ApiResponse.Apprenticeship.HasHadDataLockSuccess = hasHadDataLockSuccess;
                return this;
            }

            public DetailsViewModelMapperFixture WithChangeOfPartyRequest(ChangeOfPartyRequestType requestType, ChangeOfPartyRequestStatus status, Party? withParty = null)
            {
                var newApprenticeshipId = Fixture.Create<long>();

                ApiResponse.ChangeOfPartyRequests = new List<GetManageApprenticeshipDetailsResponse.ChangeOfPartyRequest>
                    {
                        new GetManageApprenticeshipDetailsResponse.ChangeOfPartyRequest
                        {
                            Id = 1,
                            ChangeOfPartyType = requestType,
                            OriginatingParty = requestType == ChangeOfPartyRequestType.ChangeEmployer ? Party.Provider : Party.Employer,
                            Status = status,
                            WithParty = withParty,
                            NewApprenticeshipId = newApprenticeshipId
                        }
                    };

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == newApprenticeshipId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNewApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithChangeOfEmployerChain()
            {
                var newApprenticeshipId = Fixture.Create<long>();

                ApiResponse.ChangeOfEmployerChain = new List<GetManageApprenticeshipDetailsResponse.ChangeOfEmployerLink>
                    {
                        new GetManageApprenticeshipDetailsResponse.ChangeOfEmployerLink
                        {
                            ApprenticeshipId = newApprenticeshipId,
                            EmployerName = Fixture.Create<string>(),
                            StartDate = Fixture.Create<DateTime>(),
                            EndDate = Fixture.Create<DateTime>(),
                            StopDate = Fixture.Create<DateTime>(),
                            CreatedOn = Fixture.Create<DateTime>()
                        }
                    };

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == newApprenticeshipId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNewApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithPreviousApprenticeship(bool sameProvider)
            {
                ApiResponse.Apprenticeship.ContinuationOfId = Fixture.Create<long>();
                ApiResponse.Apprenticeship.PreviousProviderId = sameProvider ? ApiResponse.Apprenticeship.ProviderId : ApiResponse.Apprenticeship.ProviderId + 1;

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == ApiResponse.Apprenticeship.ContinuationOfId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedPreviousApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithoutPreviousApprenticeship()
            {
                ApiResponse.Apprenticeship.ContinuationOfId = null;
                ApiResponse.Apprenticeship.PreviousProviderId = null;
                return this;
            }

            public DetailsViewModelMapperFixture WithNextApprenticeship()
            {
                ApiResponse.Apprenticeship.ContinuedById = Fixture.Create<long>();

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == ApiResponse.Apprenticeship.ContinuedById), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNextApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithoutNextApprenticeship()
            {
                ApiResponse.Apprenticeship.ContinuedById = null;
                return this;
            }

            public DetailsViewModelMapperFixture WithEmailPopulated(string email)
            {
                ApiResponse.Apprenticeship.Email = email;
                return this;
            }

            public DetailsViewModelMapperFixture WithEmailShouldBePresentPopulated(bool present)
            {
                ApiResponse.Apprenticeship.EmailShouldBePresent =present;
                return this;
            }

            public DetailsViewModelMapperFixture WithIsOnFlexiPaymentPilotPopulated(bool isOnPilot)
            {
                ApiResponse.Apprenticeship.IsOnFlexiPaymentPilot = isOnPilot;
                return this;
            }
            public DetailsViewModelMapperFixture WithoutPendingPriceChangePopulated()
            {
                ApiResponse.PendingPriceChange = null;
                return this;
            }

            public DetailsViewModelMapperFixture WithPendingPriceChangePopulated()
            {
                ApiResponse.PendingPriceChange = new GetManageApprenticeshipDetailsResponse.PendingPriceChangeDetails
                {
                    Cost = 12324,
                    EndPointAssessmentPrice = 43258,
                    TrainingPrice = 3248
                };
                return this;
            }
        }
    }
}