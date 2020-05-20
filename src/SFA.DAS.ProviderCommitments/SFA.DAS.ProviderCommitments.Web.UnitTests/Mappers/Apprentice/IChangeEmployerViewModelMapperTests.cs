using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class IChangeEmployerViewModelMapperTests
    {
        private IChangeEmployerViewModelMapperTestsFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new IChangeEmployerViewModelMapperTestsFixture();
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
            _fixture.WithChangeOfPartRequest(status);
            await _fixture.Act();
            _fixture.VerifyResult<InformViewModel>();
        }

        [TestCase(ChangeOfPartyRequestStatus.Approved)]
        [TestCase(ChangeOfPartyRequestStatus.Pending)]
        public async Task Then_With_A_ChangeOfPartyRequest_Pending_Or_Approved_Then_Result_Is_ChangeEmployerRequestDetailsViewModel(ChangeOfPartyRequestStatus status)
        {
            _fixture.WithChangeOfPartRequest(status);
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
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ProviderIdIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ApprenticeshipHashedIdIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_ApprenticeshipIdIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_EmployerNameIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.EmployerName, result.EmployerName);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_StartDateIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.StartDate, result.StartDate);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_PriceIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Price, result.Price);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentEmployerNameIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.EmployerName, result.CurrentEmployerName);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentPriceIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.PriceEpisodes.PriceEpisodes.GetPrice(), result.CurrentPrice);
        }

        [Test]
        public async Task Then_With_ChangeEmployerRequestDetailsViewModel_CurrentStartDateIsMapped()
        {
            _fixture.WithChangeOfPartRequest(ChangeOfPartyRequestStatus.Pending);
            await _fixture.Act();
            var result = _fixture.VerifyResult<ChangeEmployerRequestDetailsViewModel>();
            Assert.AreEqual(_fixture.Apprenticeship.StartDate, result.CurrentStartDate);
        }
    }

    internal class IChangeEmployerViewModelMapperTestsFixture : Fixture
    {
        private readonly ChangeEmployerRequest _changeEmployerRequest;
        private readonly IChangeEmployerViewModelMapper _sut;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClient;

        public long ApprenticeshipId { get; set; }
        public long ProviderId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string EmployerName { get; private set; }
        public DateTime StartDate { get; private set; }
        public int Price { get; private set; }
        public GetApprenticeshipResponse Apprenticeship { get; private set; }
        public GetPriceEpisodesResponse PriceEpisodes { get; private set; }
        public GetChangeOfPartyRequestsResponse ChangeOfPartyRequests { get; private set; }
        public IChangeEmployerViewModel Result { get; private set; }

        public IChangeEmployerViewModelMapperTestsFixture()
        {
            var autoFixture = new Fixture();

            ProviderId = 123;
            ApprenticeshipId = 234;
            ApprenticeshipHashedId = "SD23DS24";
            EmployerName = autoFixture.Create<string>();
            StartDate = autoFixture.Create<DateTime>();
            Price = autoFixture.Create<int>();

            _changeEmployerRequest = new ChangeEmployerRequest
            {
                ApprenticeshipId = ApprenticeshipId,
                ProviderId = ProviderId,
                ApprenticeshipHashedId = ApprenticeshipHashedId
            };

            Apprenticeship = new GetApprenticeshipResponse
            {
                EmployerName = autoFixture.Create<string>(),
                StartDate = autoFixture.Create<DateTime>()
            };

            PriceEpisodes = new GetPriceEpisodesResponse
            {
                PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                {
                    new GetPriceEpisodesResponse.PriceEpisode
                    {
                        FromDate = DateTime.MinValue,
                        Cost = autoFixture.Create<int>()
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

            _sut = new IChangeEmployerViewModelMapper(_commitmentsApiClient.Object);
        }

        public IChangeEmployerViewModelMapperTestsFixture WithChangeOfPartRequest(ChangeOfPartyRequestStatus? requestStatus)
        {
            if (requestStatus.HasValue)
            {
                ChangeOfPartyRequests = new GetChangeOfPartyRequestsResponse
                    {ChangeOfPartyRequests = new List<GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest>
                    {
                        new GetChangeOfPartyRequestsResponse.ChangeOfPartyRequest
                        {
                            ChangeOfPartyType = ChangeOfPartyRequestType.ChangeEmployer,
                            OriginatingParty = Party.Provider,
                            Status = requestStatus.Value,
                            EmployerName = EmployerName,
                            Price = Price,
                            StarDate = StartDate
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
            Result = await _sut.Map(_changeEmployerRequest);
            return Result;
        }

        public T VerifyResult<T>()
        {
            Assert.IsInstanceOf<T>(Result);
            return (T) Result;
        }
    }
}