using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class ChangeEmployerViewModelMapperTests
    {
        private ChangeEmployerViewModelMapperTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ChangeEmployerViewModelMapperTestsFixture();
        }

        [Test]
        public async Task Then_With_No_ChangeOfPartyRequest_Pending_Then_Result_Is_InformViewModel()
        {
            await _fixture.Act();
            _fixture.VerifyResult<InformViewModel>();
        }

        [TestCase(ChangeOfPartyRequestStatus.Rejected)]
        [TestCase(ChangeOfPartyRequestStatus.Withdrawn)]
        public async Task Then_With_A_ChangeOfPartyRequest_Rejected_Or_Withdrawn_Then_Result_Is_InformViewModel(ChangeOfPartyRequestStatus status)
        {
            _fixture.WithChangeOfPartyRequest(status);
            await _fixture.Act();
            _fixture.VerifyResult<InformViewModel>();
        }

        [TestCase(ChangeOfPartyRequestStatus.Approved)]
        [TestCase(ChangeOfPartyRequestStatus.Pending)]
        public async Task Then_With_A_ChangeOfPartyRequest_Pending_Or_Approved_Then_Result_Is_ChangeEmployerRequestDetailsViewModel(ChangeOfPartyRequestStatus status)
        {
            _fixture.WithChangeOfPartyRequest(status);
            await _fixture.Act();
            _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
        }

        [Test]
        public async Task Then_With_InformViewModel_ProviderIdIsMapped()
        {
            await _fixture.Act();

            var result = _fixture.VerifyResult<InformViewModel>();
            Assert.AreEqual(_fixture.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task Then_With_InformViewModel_ApprenticeshipIdIsMapped()
        {
            await _fixture.Act();
            var result = _fixture.VerifyResult<InformViewModel>();

            Assert.AreEqual(_fixture.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task Then_With_InformViewModel_ApprenticeshipHashedIdIsMapped()
        {
            await _fixture.Act();
            var result = _fixture.VerifyResult<InformViewModel>();
            Assert.AreEqual(_fixture.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task Then_With_InformViewModel_LegalEntityNameIsMapped()
        {
            await _fixture.Act();
            var result = _fixture.VerifyResult<InformViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.EmployerName, result.LegalEntityName);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ProviderIdIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ApprenticeshipHashedIdIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ApprenticeshipIdIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_EmployerNameIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.EmployerName, result.EmployerName);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_StartDateIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.StartDate, result.StartDate);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_EndDateIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.EndDate, result.EndDate);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_PriceIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Price, result.Price);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentEmployerNameIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.EmployerName, result.CurrentEmployerName);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentPriceIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.PriceEpisodes.PriceEpisodes.GetPrice(), result.CurrentPrice);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentStartDateIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.StartDate, result.CurrentStartDate);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentEndDateIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.EndDate, result.CurrentEndDate);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CohortIdIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ChangeOfPartyRequests.ChangeOfPartyRequests.First().CohortId, result.CohortId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CohortReferenceIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_WithPartyIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ChangeOfPartyRequests.ChangeOfPartyRequests.First().WithParty, result.WithParty);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_StatusIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Approved);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ChangeOfPartyRequests.ChangeOfPartyRequests.First().Status, result.Status);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_EncodedNewApprenticeshipIdIsMapped()
        {
            _fixture.WithChangeOfPartyRequest(ChangeOfPartyRequestStatus.Approved);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.EncodedNewApprenticeshipId, result.EncodedNewApprenticeshipId);
        }
    }

    internal class ChangeEmployerViewModelMapperTestsFixture : Fixture
    {
        private readonly ChangeEmployerRequest _changeEmployerRequest;
        private readonly IChangeEmployerViewModelMapper _sut;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private IChangeEmployerViewModel _result;
        private readonly Fixture _autoFixture;

        public long ApprenticeshipId { get; }
        public long ProviderId { get; }
        public string ApprenticeshipHashedId { get; }
        public string EmployerName { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int Price { get; }
        public GetApprenticeshipResponse Apprenticeship { get; }
        public GetPriceEpisodesResponse PriceEpisodes { get; }
        public GetChangeOfPartyRequestsResponse ChangeOfPartyRequests { get; private set; }
        
        public string CohortReference { get; }
        public string EncodedNewApprenticeshipId { get; }

        public ChangeEmployerViewModelMapperTestsFixture()
        {
            _autoFixture = new Fixture();

            ProviderId = 123;
            ApprenticeshipId = 234;
            ApprenticeshipHashedId = "SD23DS24";
            EmployerName = _autoFixture.Create<string>();
            StartDate = _autoFixture.Create<DateTime>();
            EndDate = _autoFixture.Create<DateTime>();
            Price = _autoFixture.Create<int>();
            CohortReference = _autoFixture.Create<string>();
            EncodedNewApprenticeshipId = _autoFixture.Create<string>();

            _changeEmployerRequest = new ChangeEmployerRequest
            {
                ApprenticeshipId = ApprenticeshipId,
                ProviderId = ProviderId,
                ApprenticeshipHashedId = ApprenticeshipHashedId
            };

            Apprenticeship = new GetApprenticeshipResponse
            {
                EmployerName = _autoFixture.Create<string>(),
                StartDate = _autoFixture.Create<DateTime>()
            };

            PriceEpisodes = new GetPriceEpisodesResponse
            {
                PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                {
                    new()
                    {
                        FromDate = DateTime.MinValue,
                        Cost = _autoFixture.Create<int>()
                    }
                }
            };

            ChangeOfPartyRequests = new GetChangeOfPartyRequestsResponse
                { ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>() };

            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient
                .Setup(x => x.GetChangeOfPartyRequests(It.Is<long>(a => a == ApprenticeshipId),
                    It.IsAny<CancellationToken>())).ReturnsAsync(ChangeOfPartyRequests);

            _commitmentsApiClient.Setup(x => x.GetApprenticeship(It.Is<long>(a => a == ApprenticeshipId),
                It.IsAny<CancellationToken>())).ReturnsAsync(Apprenticeship);

            _commitmentsApiClient.Setup(x => x.GetPriceEpisodes(It.Is<long>(a => a == ApprenticeshipId),
                It.IsAny<CancellationToken>())).ReturnsAsync(PriceEpisodes);

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference))
                .Returns(CohortReference);

            encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId))
                .Returns(EncodedNewApprenticeshipId);

            _sut = new IChangeEmployerViewModelMapper(_commitmentsApiClient.Object, encodingService.Object);
        }

        public ChangeEmployerViewModelMapperTestsFixture WithChangeOfPartyRequest(ChangeOfPartyRequestStatus? requestStatus)
        {
            if (requestStatus.HasValue)
            {
                ChangeOfPartyRequests = new GetChangeOfPartyRequestsResponse
                    {ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>
                    {
                        new()
                        {
                            ChangeOfPartyType = ChangeOfPartyRequestType.ChangeEmployer,
                            OriginatingParty = Party.Provider,
                            Status = requestStatus.Value,
                            EmployerName = EmployerName,
                            Price = Price,
                            StartDate = StartDate,
                            EndDate = EndDate,
                            CohortId = _autoFixture.Create<long>(),
                            WithParty = _autoFixture.Create<Party>(),
                            NewApprenticeshipId = requestStatus == ChangeOfPartyRequestStatus.Approved
                                ? _autoFixture.Create<long>()
                                : default(long?)
                        }
                    }};
            }
            else
            {
                ChangeOfPartyRequests = new GetChangeOfPartyRequestsResponse
                    { ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>() };
            }

            _commitmentsApiClient
                .Setup(x => x.GetChangeOfPartyRequests(It.Is<long>(a => a == ApprenticeshipId),
                    It.IsAny<CancellationToken>())).ReturnsAsync(ChangeOfPartyRequests);

            return this;
        }

        public async Task<IChangeEmployerViewModel> Act()
        {
            _result = await _sut.Map(_changeEmployerRequest);
            return _result;
        }

        public T VerifyResult<T>()
        {
            Assert.IsInstanceOf<T>(_result);
            return (T) _result;
        }
    }
}