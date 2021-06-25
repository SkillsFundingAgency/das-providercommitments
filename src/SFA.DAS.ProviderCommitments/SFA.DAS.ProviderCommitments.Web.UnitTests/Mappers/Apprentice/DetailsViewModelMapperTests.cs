using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
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
            Assert.AreEqual(_fixture.ApiResponse.FirstName + " " + _fixture.ApiResponse.LastName, _fixture.Result.ApprenticeName);
        }

        [Test]
        public async Task ThenEmployerIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.EmployerName, _fixture.Result.Employer);
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
            Assert.AreEqual(_fixture.ApiResponse.Status, _fixture.Result.Status);
        }

        [Test]
        public async Task ThenStopDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.StopDate, _fixture.Result.StopDate);
        }

        [Test]
        public async Task ThenPauseDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.PauseDate, _fixture.Result.PauseDate);
        }

        [Test]
        public async Task ThenCompletionDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.CompletionDate, _fixture.Result.CompletionDate);
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
            Assert.AreEqual(_fixture.ApiResponse.DateOfBirth, _fixture.Result.DateOfBirth);
        }

        [Test]
        public async Task ThenUlnIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.Uln, _fixture.Result.Uln);
        }

        [Test]
        public async Task ThenCourseNameIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.CourseName, _fixture.Result.CourseName);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.StartDate, _fixture.Result.StartDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.EndDate, _fixture.Result.EndDate);
        }

        [Test]
        public async Task ThenProviderRefIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.ApiResponse.ProviderReference, _fixture.Result.ProviderRef);
        }

        [Test]
        public async Task ThenPriceIsMappedCorrectly()
        {
            await _fixture.Map();
            Assert.AreEqual(_fixture.PriceEpisodesApiResponse.PriceEpisodes.First().Cost, _fixture.Result.Cost);
        }

        [TestCase(ApprenticeshipStatus.Live, true)]
        [TestCase(ApprenticeshipStatus.Paused, true)]
        [TestCase(ApprenticeshipStatus.WaitingToStart, true)]
        [TestCase(ApprenticeshipStatus.Stopped, false)]
        [TestCase(ApprenticeshipStatus.Completed, false)]
        public async Task ThenAllowEditApprenticeIsMappedCorrectly(ApprenticeshipStatus status, bool expectedAllowEditApprentice)
        {
            _fixture.WithApprenticeshipStatus(status);

            await _fixture.Map();

            Assert.AreEqual(expectedAllowEditApprentice, _fixture.Result.AllowEditApprentice);
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

        [TestCase(null, false)]
        [TestCase(ChangeOfPartyRequestStatus.Approved, false)]
        [TestCase(ChangeOfPartyRequestStatus.Rejected, false)]
        [TestCase(ChangeOfPartyRequestStatus.Withdrawn, false)]
        [TestCase(ChangeOfPartyRequestStatus.Pending, true)]
        public async Task ThenHasChangeOfProviderRequestPendingIsMappedCorrectly(ChangeOfPartyRequestStatus? status, bool expectHasPending)
        {
            if (status.HasValue)
            {
                _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeProvider, status.Value);
            }

            await _fixture.Map();

            Assert.AreEqual(expectHasPending, _fixture.Result.HasPendingChangeOfProviderRequest);
        }

        [TestCase(Party.Employer)]
        [TestCase(Party.Provider)]
        public async Task ThenPendingChangeOfPartyRequestWithPartyIsMappedCorrectly(Party withParty)
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Pending, withParty);
            await _fixture.Map();
            Assert.AreEqual(withParty, _fixture.Result.PendingChangeOfPartyRequestWithParty);
        }

        [TestCase(Party.Employer)]
        [TestCase(Party.Provider)]
        public async Task ThenApprovedChangeOfPartyRequestWithPartyIsMappedCorrectly(Party withParty)
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Approved, withParty);
            await _fixture.Map();
            Assert.IsTrue(_fixture.Result.HasApprovedChangeOfPartyRequest);
        }

        [TestCase(Party.Employer)]
        [TestCase(Party.Provider)]
        public async Task ThenEncodedNewApprenticeshipIdIsMappedCorrectly(Party withParty)
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Approved, withParty);
            await _fixture.Map();
            Assert.AreEqual(_fixture.EncodedNewApprenticeshipId, _fixture.Result.EncodedNewApprenticeshipId);
        }

        [Test]
        public async Task ThenEncodedPreviousApprenticeshipIdIsMappedCorrectly()
        {
            _fixture.WithPreviousApprenticeship(true);
            await _fixture.Map();
            Assert.AreEqual(_fixture.EncodedPreviousApprenticeshipId, _fixture.Result.EncodedPreviousApprenticeshipId);
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

        [Test]
        public async Task ThenAPendingChangeOfPartyOriginatingFromProviderDoesNotSetHasPendingChangeOfProviderRequest()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, ChangeOfPartyRequestStatus.Pending);

            await _fixture.Map();

            Assert.IsFalse(_fixture.Result.HasPendingChangeOfProviderRequest);
        }

        [TestCase(ChangeOfPartyRequestStatus.Approved, false)]
        [TestCase(ChangeOfPartyRequestStatus.Pending, false)]
        [TestCase(ChangeOfPartyRequestStatus.Rejected, true)]
        [TestCase(ChangeOfPartyRequestStatus.Withdrawn, true)]
        public async Task ThenPendingOrApprovedChangeOfPartyRequestPreventsChangeOfEmployer(ChangeOfPartyRequestStatus status, bool expectChangeEmployerEnabled)
        {
            _fixture                
                .WithChangeOfPartyRequest(ChangeOfPartyRequestType.ChangeEmployer, status);
            await _fixture.Map();
            Assert.AreEqual(expectChangeEmployerEnabled, _fixture.Result.IsChangeOfEmployerEnabled);
        }

        [Test]
        public async Task ThenIfChangeOfEmployerChainThenEmployerHistoryIsMappedCorrectly()
        {
            _fixture.WithChangeOfEmployerChain();
            await _fixture.Map();
            Assert.IsNotNull(_fixture.Result.EmployerHistory);
        }

        public class DetailsViewModelMapperFixture
        {
            private DetailsViewModelMapper _sut;
            public DetailsRequest Source { get; }
            public DetailsViewModel Result { get; private set; }
            public GetApprenticeshipResponse ApiResponse { get; }
            public GetPriceEpisodesResponse PriceEpisodesApiResponse { get; }
            public GetApprenticeshipUpdatesResponse GetApprenticeshipUpdatesResponse { get; private set; }
            public GetDataLocksResponse GetDataLocksResponse { get; private set; }
            public GetChangeOfPartyRequestsResponse GetChangeOfPartyRequestsResponse { get; private set; }
            public GetChangeOfEmployerChainResponse GetChangeOfEmployerChainResponse { get; private set; }

            private readonly Mock<IEncodingService> _encodingService;            
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
                ApiResponse = Fixture.Create<GetApprenticeshipResponse>();
                ApiResponse.ProviderId = Source.ProviderId;
                CohortReference = Fixture.Create<string>();
                AgreementId = Fixture.Create<string>();
                URL = Fixture.Create<string>();
                PriceEpisodesApiResponse = new GetPriceEpisodesResponse
                {
                    PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                    {
                        new GetPriceEpisodesResponse.PriceEpisode {Cost = 100, FromDate = DateTime.UtcNow}
                    }
                };

                GetApprenticeshipUpdatesResponse = new GetApprenticeshipUpdatesResponse
                {
                    ApprenticeshipUpdates = new List<GetApprenticeshipUpdatesResponse.ApprenticeshipUpdate>()
                };

                GetDataLocksResponse = new GetDataLocksResponse
                {
                    DataLocks = new List<DataLock>()
                };

                GetChangeOfPartyRequestsResponse = new GetChangeOfPartyRequestsResponse
                {
                    ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>()
                };

                GetChangeOfEmployerChainResponse = new GetChangeOfEmployerChainResponse
                {
                    ChangeOfEmployerChain = new List<GetChangeOfEmployerChainResponse.ChangeOfEmployerLink>()
                };

                _encodingService = new Mock<IEncodingService>();
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns(CohortReference);
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns(AgreementId);

                EncodedNewApprenticeshipId = Fixture.Create<string>();
                EncodedPreviousApprenticeshipId = Fixture.Create<string>();
            }

            public async Task<DetailsViewModelMapperFixture> Map()
            {
                var apiClient = new Mock<ICommitmentsApiClient>();
                apiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ApiResponse);

                apiClient.Setup(x => x.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(PriceEpisodesApiResponse);

                apiClient.Setup(x => x.GetApprenticeshipUpdates(It.IsAny<long>(), It.IsAny<GetApprenticeshipUpdatesRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => GetApprenticeshipUpdatesResponse);

                apiClient.Setup(x => x.GetApprenticeshipDatalocksStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetDataLocksResponse);

                apiClient.Setup(x => x.GetChangeOfPartyRequests(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetChangeOfPartyRequestsResponse);

                apiClient.Setup(x => x.GetChangeOfEmployerChain(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetChangeOfEmployerChainResponse);

                _sut = new DetailsViewModelMapper(apiClient.Object, _encodingService.Object, Mock.Of<ILogger<DetailsViewModelMapper>>());

                Result = await _sut.Map(Source);
                return this;
            }

            public DetailsViewModelMapperFixture WithApprenticeshipStatus(
                ApprenticeshipStatus status)
            {
                ApiResponse.Status = status;
                return this;
            }

            public DetailsViewModelMapperFixture WithPendingUpdatesForProvider()
            {
                GetApprenticeshipUpdatesResponse = new GetApprenticeshipUpdatesResponse
                {
                    ApprenticeshipUpdates = new List<GetApprenticeshipUpdatesResponse.ApprenticeshipUpdate>()
                    {
                        new GetApprenticeshipUpdatesResponse.ApprenticeshipUpdate
                        {
                            Id = 1,
                            OriginatingParty = Party.Provider
                        }
                    }
                };
                return this;
            }

            public DetailsViewModelMapperFixture WithPendingUpdatesForEmployer()
            {
                GetApprenticeshipUpdatesResponse = new GetApprenticeshipUpdatesResponse
                {
                    ApprenticeshipUpdates = new List<GetApprenticeshipUpdatesResponse.ApprenticeshipUpdate>()
                    {
                        new GetApprenticeshipUpdatesResponse.ApprenticeshipUpdate
                        {
                            Id = 1,
                            OriginatingParty = Party.Employer                        }
                    }
                };
                return this;
            }

            public DetailsViewModelMapperFixture WithResolvedDataLocks()
            {
                GetDataLocksResponse.DataLocks = new List<DataLock> { 
                    new DataLock
                    {
                        Id = 1,
                        TriageStatus = TriageStatus.Unknown,
                        DataLockStatus = Status.Fail,
                        IsResolved = true
                    },
                    new DataLock
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
                GetDataLocksResponse.DataLocks = new List<DataLock> { new DataLock
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
                var dataLocks = GetDataLocksResponse.DataLocks.ToList();
                dataLocks.Add(new DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Fail,
                    IsResolved = false,
                    ErrorCode = errorCode
                });

                GetDataLocksResponse.DataLocks = dataLocks.AsReadOnly();
                return this;
            }

            public DetailsViewModelMapperFixture WithUnResolvedAndPassingDataLocks()
            {
                GetDataLocksResponse.DataLocks = new List<DataLock> { new DataLock
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
                GetDataLocksResponse.DataLocks = new List<DataLock> { new DataLock
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
                ApiResponse.HasHadDataLockSuccess = hasHadDataLockSuccess;
                return this;
            }           

            public DetailsViewModelMapperFixture WithChangeOfPartyRequest(ChangeOfPartyRequestType requestType, ChangeOfPartyRequestStatus status, Party? withParty = null)
            {
                var newApprenticeshipId = Fixture.Create<long>();

                GetChangeOfPartyRequestsResponse = new GetChangeOfPartyRequestsResponse
                {
                    ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>
                    {
                        new GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest
                        {
                            Id = 1,
                            ChangeOfPartyType = requestType,
                            OriginatingParty = requestType == ChangeOfPartyRequestType.ChangeEmployer ? Party.Provider : Party.Employer,
                            Status = status,
                            WithParty = withParty,
                            NewApprenticeshipId = newApprenticeshipId
                        }
                    }
                };

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == newApprenticeshipId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNewApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithChangeOfEmployerChain()
            {
                var newApprenticeshipId = Fixture.Create<long>();

                GetChangeOfEmployerChainResponse = new GetChangeOfEmployerChainResponse
                {
                    ChangeOfEmployerChain = new List<GetChangeOfEmployerChainResponse.ChangeOfEmployerLink>
                    {
                        new GetChangeOfEmployerChainResponse.ChangeOfEmployerLink
                        {
                            ApprenticeshipId = newApprenticeshipId,
                            EmployerName = Fixture.Create<string>(),
                            StartDate = Fixture.Create<DateTime>(),
                            EndDate = Fixture.Create<DateTime>(),
                            StopDate = Fixture.Create<DateTime>(),
                            CreatedOn = Fixture.Create<DateTime>()
                        }
                    }
                };

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == newApprenticeshipId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNewApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithPreviousApprenticeship(bool sameProvider)
            {
                ApiResponse.ContinuationOfId = Fixture.Create<long>();
                ApiResponse.PreviousProviderId = sameProvider ? ApiResponse.ProviderId : ApiResponse.ProviderId + 1;

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == ApiResponse.ContinuationOfId), EncodingType.ApprenticeshipId))
                    .Returns(EncodedPreviousApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithoutPreviousApprenticeship()
            {
                ApiResponse.ContinuationOfId = null;
                ApiResponse.PreviousProviderId = null;
                return this;
            }

            public DetailsViewModelMapperFixture WithNextApprenticeship()
            {
                ApiResponse.ContinuedById = Fixture.Create<long>();

                _encodingService.Setup(x => x.Encode(It.Is<long>(id => id == ApiResponse.ContinuedById), EncodingType.ApprenticeshipId))
                    .Returns(EncodedNextApprenticeshipId);

                return this;
            }

            public DetailsViewModelMapperFixture WithoutNextApprenticeship()
            {
                ApiResponse.ContinuedById = null;
                return this;
            }
        }
    }
}