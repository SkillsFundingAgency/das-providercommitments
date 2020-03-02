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
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeDetailsRequestToViewModelMapperTests
{
    [TestFixture]
    public class WhenIMapApprenticeDetailsRequestToViewModel
    {
        private WhenIMapApprenticeDetailsRequestToViewModelFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenIMapApprenticeDetailsRequestToViewModelFixture();
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
            Assert.AreEqual(_fixture.ApiResponse.Reference, _fixture.Result.ProviderRef);
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

        public class WhenIMapApprenticeDetailsRequestToViewModelFixture
        {
            private readonly DetailsViewModelMapper _mapper;
            public DetailsRequest Source { get; }
            public DetailsViewModel Result { get; private set; }
            public GetApprenticeshipResponse ApiResponse { get; }
            public GetPriceEpisodesResponse PriceEpisodesApiResponse { get; }
            public GetApprenticeshipUpdatesResponse GetApprenticeshipUpdatesResponse { get; private set; }
            public GetDataLocksResponse GetDataLocksResponse { get; private set; }

            private readonly Mock<IEncodingService> _encodingService;
            public string CohortReference { get; }
            public string AgreementId { get; }
            public string URL { get; }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture()
            {
                var fixture = new Fixture();
                Source = fixture.Create<DetailsRequest>();
                ApiResponse = fixture.Create<GetApprenticeshipResponse>();
                CohortReference = fixture.Create<string>();
                AgreementId = fixture.Create<string>();
                URL = fixture.Create<string>();
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
                    DataLocks = new List<GetDataLocksResponse.DataLock>()
                };

                _encodingService = new Mock<IEncodingService>();
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns(CohortReference);
                _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.PublicAccountLegalEntityId)).Returns(AgreementId);

                var apiClient = new Mock<ICommitmentsApiClient>();
                apiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ApiResponse);

                apiClient.Setup(x => x.GetPriceEpisodes(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(PriceEpisodesApiResponse);

                apiClient.Setup(x => x.GetApprenticeshipUpdates(It.IsAny<long>(), It.IsAny<GetApprenticeshipUpdatesRequest>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => GetApprenticeshipUpdatesResponse);
                
                apiClient.Setup(x => x.GetApprenticeshipDatalocksStatus(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetDataLocksResponse);

                _mapper = new DetailsViewModelMapper(apiClient.Object, _encodingService.Object, Mock.Of<ILogger<DetailsViewModelMapper>>());
            }

            public async Task<WhenIMapApprenticeDetailsRequestToViewModelFixture> Map()
            {
                Result = await _mapper.Map(Source);
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithApprenticeshipStatus(
                ApprenticeshipStatus status)
            {
                ApiResponse.Status = status;
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithPendingUpdatesForProvider()
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

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithPendingUpdatesForEmployer()
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

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithResolvedDataLocks()
            {
                GetDataLocksResponse.DataLocks = new List<GetDataLocksResponse.DataLock> { 
                    new GetDataLocksResponse.DataLock
                    {
                        Id = 1,
                        TriageStatus = TriageStatus.Unknown,
                        DataLockStatus = Status.Fail,
                        IsResolved = true
                    },
                    new GetDataLocksResponse.DataLock
                    {
                        Id = 2,
                        TriageStatus = TriageStatus.Unknown,
                        DataLockStatus = Status.Pass,
                        IsResolved = true
                    }
                };
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithUnresolvedAndFailedDataLocks(DataLockErrorCode errorCode = DataLockErrorCode.Dlock07)
            {
                GetDataLocksResponse.DataLocks = new List<GetDataLocksResponse.DataLock> { new GetDataLocksResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Fail,
                    IsResolved = false,
                    ErrorCode = errorCode
                }};
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithAnotherDataLock(DataLockErrorCode errorCode)
            {
                var dataLocks = GetDataLocksResponse.DataLocks.ToList();
                dataLocks.Add(new GetDataLocksResponse.DataLock
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

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithUnResolvedAndPassingDataLocks()
            {
                GetDataLocksResponse.DataLocks = new List<GetDataLocksResponse.DataLock> { new GetDataLocksResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = TriageStatus.Unknown,
                    DataLockStatus = Status.Pass,
                    IsResolved = false
                }};
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithUnResolvedDataLocksInTriage(TriageStatus triageStatus)
            {
                GetDataLocksResponse.DataLocks = new List<GetDataLocksResponse.DataLock> { new GetDataLocksResponse.DataLock
                {
                    Id = 1,
                    TriageStatus = triageStatus,
                    DataLockStatus = Status.Fail,
                    IsResolved = false
                }};
                return this;
            }

            public WhenIMapApprenticeDetailsRequestToViewModelFixture WithHasHadDataLockSuccess(bool hasHadDataLockSuccess)
            {
                ApiResponse.HasHadDataLockSuccess = hasHadDataLockSuccess;
                return this;
            }
        }
    }
}