using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class PriceRequestMapperTests
    {
        private PriceRequestMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new PriceRequestMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            _fixture.WithValidStopDate();

            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenProviderIdIsMapped()
        {
            _fixture.WithValidStopDate();

            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            _fixture.WithValidStopDate();

            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.EmployerAccountLegalEntityPublicHashedId, result.EmployerAccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenStartDateIsMapped()
        {
            _fixture.WithValidStopDate();

            var result = await _fixture.Act();

            Assert.AreEqual(_fixture.ViewModel.StartDate.MonthYear, result.NewStartDate);
        }

        [Test]
        public void AndStartDateIsBeforeStopDate_ThenValidationExceptionIsThrown()
        {
            _fixture
                 .WithValidStopDate()
                 .WithStartDateBeforeStopDate();

            Assert.ThrowsAsync<ValidationException>(() => _fixture.Act());
        }
    }

    public class PriceRequestMapperFixture
    {
        private readonly PriceRequestMapper _sut;
        private readonly Mock<ICommitmentsApiClient> _commitmentsApiClientMock;

        public DatesViewModel ViewModel { get; }

        public PriceRequestMapperFixture()
        {
            ViewModel = new DatesViewModel
            {
                ApprenticeshipHashedId = "DFE546SD",
                ProviderId = 2350,
                EmployerAccountLegalEntityPublicHashedId = "DFE348FD",
                StopDate = DateTime.UtcNow.AddDays(-5),
                StartMonth = 6,
                StartYear = 2020,
            };
            _commitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            _sut = new PriceRequestMapper(_commitmentsApiClientMock.Object);
        }

        public Task<PriceRequest> Act() => _sut.Map(ViewModel);

        public PriceRequestMapperFixture WithValidStopDate()
        {
            _commitmentsApiClientMock
                .Setup(x => x.GetApprenticeship(ViewModel.ApprenticeshipId, default(CancellationToken)))
                .ReturnsAsync(new GetApprenticeshipResponse
                {
                    StopDate = new DateTime(2018, 1, 1)
                });
            return this;
        }

        public PriceRequestMapperFixture WithStartDateBeforeStopDate()
        {
            ViewModel.StartDate.Month = 1;
            ViewModel.StartDate.Year = 2010;
            return this;
        }
    }
}