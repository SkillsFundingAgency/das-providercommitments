using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ConfirmViewModelMapperTests
    {
        private ConfirmViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ConfirmViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.Request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.EncodedAccountLegalEntityId, result.AccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenOldEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.GetApprenticeshipResponse.EmployerName, result.OldEmployerName);
        }

        [Test]
        public async Task ThenApprenticeNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual($"{_fixture.GetApprenticeshipResponse.FirstName} {_fixture.GetApprenticeshipResponse.LastName}", result.ApprenticeName);
        }

        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.GetApprenticeshipResponse.StopDate, result.StopDate);
        }

        [Test]
        public async Task ThenOldStartDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.GetApprenticeshipResponse.StartDate, result.OldStartDate);
        }

        [Test]
        public async Task ThenOldEndDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.GetApprenticeshipResponse.EndDate, result.OldEndDate);
        }


        [Test]
        public async Task ThenOldPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.PriceEpisodesResponse.PriceEpisodes.First().Cost, result.OldPrice);
        }

        [Test]
        public async Task ThenNewEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.AccountLegalEntityResponse.LegalEntityName, result.NewEmployerName);
        }

        [Test]
        public async Task ThenNewStartDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedStartDate = new MonthYearModel(_fixture.CacheItem.StartDate);

            Assert.AreEqual(expectedStartDate.MonthYear, result.NewStartDate);
        }

        [Test]
        public async Task ThenNewEndDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedEndDate = new MonthYearModel(_fixture.CacheItem.EndDate);

            Assert.AreEqual(expectedEndDate.MonthYear, result.NewEndDate);
        }

        [Test]
        public async Task ThenNewEmploymentEndDateIsMapped()
        {
            _fixture.SetEmploymentEndDateInRequest(DateTime.Now);
            
            var result = await _fixture.Map();
            
            var expectedEndDate = new MonthYearModel(_fixture.CacheItem.EmploymentEndDate);
            Assert.AreEqual(expectedEndDate.MonthYear, result.NewEmploymentEndDate);
        }

        [Test]
        public async Task ThenNewPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.CacheItem.Price, result.NewPrice);
        }

        [Test]
        public async Task ThenNewEmploymentPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.CacheItem.EmploymentPrice, result.NewEmploymentPrice);
        }

        [Test]
        public async Task ThenFundingBandCapIsMapped()
        {
            var priceBand = 1000;
            _fixture.SetStartDateInRequest(DateTime.Now).SetPriceBand(priceBand, DateTime.Now);
            var result = await _fixture.Map();

            Assert.AreEqual(priceBand, result.FundingBandCap);
        }

        [TestCase(1000, 900, true)]
        [TestCase(800, 900, false)]
        [TestCase(800, 800, false)]
        public async Task ThenExceedsFundingBandCapIsMapped(int price, int fundingCap, bool fundingCapExceeded)
        {
            _fixture.SetPriceInRequest(price).SetStartDateInRequest(DateTime.Now).SetPriceBand(fundingCap, DateTime.Now);
            var result = await _fixture.Map();

            Assert.AreEqual(result.ExceedsFundingBandCap, fundingCapExceeded);
        }

        [Test]
        public async Task Then_ShowDeliveryModel_If_Automatically_Changed_To_Regular()
        {
            _fixture.CacheItem.SkippedDeliveryModelSelection = true;
            _fixture.CacheItem.DeliveryModel = DeliveryModel.Regular;
            _fixture.GetApprenticeshipResponse.DeliveryModel = CommitmentsV2.Types.DeliveryModel.FlexiJobAgency;
            var result = await _fixture.Map();
            Assert.IsTrue(result.ShowDeliveryModel);
        }

        [TestCase(CommitmentsV2.Types.DeliveryModel.Regular, true)]
        [TestCase(CommitmentsV2.Types.DeliveryModel.FlexiJobAgency, true)]
        [TestCase(CommitmentsV2.Types.DeliveryModel.Regular, true)]
        public async Task Then_ShowDeliveryModel_If_Manual_Selection_Was_Made(DeliveryModel selectedDeliveryModel, bool expectShowDeliveryModel)
        {
            _fixture.CacheItem.SkippedDeliveryModelSelection = false;
            _fixture.CacheItem.DeliveryModel = selectedDeliveryModel;
            var result = await _fixture.Map();
            Assert.AreEqual(expectShowDeliveryModel, result.ShowDeliveryModel);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Then_ShowDeliveryModelChangeLink_Is_False_If_Selection_Was_Skipped(bool skippedDeliveryModelSelection, bool expectShowDeliveryModel)
        {
            _fixture.CacheItem.SkippedDeliveryModelSelection = skippedDeliveryModelSelection;
            var result = await _fixture.Map();
            Assert.AreEqual(expectShowDeliveryModel, result.ShowDeliveryModelChangeLink);
        }
    }

    public class ConfirmViewModelMapperFixture
    {
        private readonly ConfirmViewModelMapper _sut;
        private TrainingProgramme _trainingProgramme;
        
        public ConfirmRequest Request { get; }
        public GetApprenticeshipResponse GetApprenticeshipResponse { get; }
        public AccountLegalEntityResponse AccountLegalEntityResponse { get; }
        public GetPriceEpisodesResponse PriceEpisodesResponse { get; }
        public ChangeEmployerCacheItem CacheItem { get; }
        public string EncodedAccountLegalEntityId { get; }

        public ConfirmViewModelMapperFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<ConfirmRequest>();
            EncodedAccountLegalEntityId = fixture.Create<string>();
            GetApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            _trainingProgramme = fixture.Create<TrainingProgramme>();
            AccountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();
            PriceEpisodesResponse = new GetPriceEpisodesResponse
            {
                PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                    {
                        new() {Cost = 100, FromDate = DateTime.UtcNow}
                    }
            };

            CacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "092023")
                .With(x => x.EndDate, "102024")
                .With(x => x.EmploymentEndDate, "102024")
                .Create();
            var cacheService = new Mock<ICacheStorageService>();
            cacheService.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(CacheItem);

            var commitmentAiClient = new Mock<ICommitmentsApiClient>();

            commitmentAiClient.Setup(x => x.GetApprenticeship(Request.ApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(() => GetApprenticeshipResponse);
            commitmentAiClient.Setup(x => x.GetAccountLegalEntity(CacheItem.AccountLegalEntityId, It.IsAny<CancellationToken>())).ReturnsAsync(() => AccountLegalEntityResponse);
            commitmentAiClient.Setup(x => x.GetPriceEpisodes(Request.ApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(() => PriceEpisodesResponse);
            commitmentAiClient
                .Setup(y => y.GetTrainingProgramme(GetApprenticeshipResponse.CourseCode, CancellationToken.None))
                .ReturnsAsync(()=> new
                    GetTrainingProgrammeResponse
                    {
                        TrainingProgramme  = _trainingProgramme
                    } );

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Encode(It.Is<long>(id => id == CacheItem.AccountLegalEntityId),
                    It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
                .Returns(EncodedAccountLegalEntityId);

            _sut = new ConfirmViewModelMapper(commitmentAiClient.Object, Mock.Of<ILogger<ConfirmViewModelMapper>>(), cacheService.Object, encodingService.Object);
        }

        public ConfirmViewModelMapperFixture SetPriceBand(int fundingCap, DateTime startDate)
        {
            _trainingProgramme = new TrainingProgramme()
            {
                FundingPeriods = new List<TrainingProgrammeFundingPeriod>
                {
                    new()
                    {
                          EffectiveFrom =startDate.AddDays(-1),
                          EffectiveTo = startDate.AddDays(1),
                          FundingCap = fundingCap
                    }
                }
            };

            return this;
        }

        public ConfirmViewModelMapperFixture SetStartDateInRequest(DateTime startDate)
        {
            CacheItem.StartDate = startDate.Month + startDate.Year.ToString();

            return this;
        }

        public ConfirmViewModelMapperFixture SetPriceInRequest(int price)
        {
            CacheItem.Price = price;
            return this;
        }

        public ConfirmViewModelMapperFixture SetEmploymentEndDateInRequest(DateTime startDate)
        {
            CacheItem.EmploymentEndDate = $"{startDate.Month}{startDate.Year}";
            return this;
        }

        public Task<ConfirmViewModel> Map() => _sut.Map(Request);
    }
}
