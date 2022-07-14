using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

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

            Assert.AreEqual(_fixture.request.ApprenticeshipHashedId, result.ApprenticeshipHashedId);
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.encodedAccountLegalEntityId, result.AccountLegalEntityPublicHashedId);
        }

        [Test]
        public async Task ThenOldEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.EmployerName, result.OldEmployerName);
        }

        [Test]
        public async Task ThenApprenticeNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual($"{_fixture.getApprenticeshipResponse.FirstName} {_fixture.getApprenticeshipResponse.LastName}", result.ApprenticeName);
        }

        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.StopDate, result.StopDate);
        }

        [Test]
        public async Task ThenOldStartDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.StartDate, result.OldStartDate);
        }

        [Test]
        public async Task ThenOldEndDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.getApprenticeshipResponse.EndDate, result.OldEndDate);
        }


        [Test]
        public async Task ThenOldPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.priceEpisodesResponse.PriceEpisodes.First().Cost, result.OldPrice);
        }

        [Test]
        public async Task ThenNewEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.accountLegalEntityResponse.LegalEntityName, result.NewEmployerName);
        }

        [Test]
        public async Task ThenNewStartDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedStartDate = new MonthYearModel(_fixture.cacheItem.StartDate);

            Assert.AreEqual(expectedStartDate.MonthYear, result.NewStartDate);
        }

        [Test]
        public async Task ThenNewEndDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedEndDate = new MonthYearModel(_fixture.cacheItem.EndDate);

            Assert.AreEqual(expectedEndDate.MonthYear, result.NewEndDate);
        }

        [Test]
        public async Task ThenNewEmploymentEndDateIsMapped()
        {
            _fixture.SetEmploymentEndDateInRequest(DateTime.Now);
            
            var result = await _fixture.Map();
            
            var expectedEndDate = new MonthYearModel(_fixture.cacheItem.EmploymentEndDate);
            Assert.AreEqual(expectedEndDate.MonthYear, result.NewEmploymentEndDate);
        }

        [Test]
        public async Task ThenNewPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.cacheItem.Price, result.NewPrice);
        }

        [Test]
        public async Task ThenNewEmploymentPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.AreEqual(_fixture.cacheItem.EmploymentPrice, result.NewEmploymentPrice);
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

        [TestCase(true, DeliveryModel.Regular, true)]
        [TestCase(true, DeliveryModel.FlexiJobAgency, false)]
        [TestCase(true, DeliveryModel.PortableFlexiJob,  false)]
        [TestCase(false, DeliveryModel.Regular, true)]
        [TestCase(false, DeliveryModel.FlexiJobAgency, true)]
        [TestCase(false, DeliveryModel.PortableFlexiJob, true)]
        public async Task Then_ShowDeliveryModel_If_Manual_Selection_Was_Made_Or_Was_Automatically_Set_To_Regular(bool skippedDeliveryModelSelection, Infrastructure.OuterApi.Types.DeliveryModel selectedDeliveryModel, bool expectShowDeliveryModel)
        {
            _fixture.cacheItem.SkippedDeliveryModelSelection = skippedDeliveryModelSelection;
            _fixture.cacheItem.DeliveryModel = selectedDeliveryModel;
            var result = await _fixture.Map();
            Assert.AreEqual(expectShowDeliveryModel, result.ShowDeliveryModel);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Then_ShowDeliveryModelChangeLink_Is_False_If_Selection_Was_Skipped(bool skippedDeliveryModelSelection, bool expectShowDeliveryModel)
        {
            _fixture.cacheItem.SkippedDeliveryModelSelection = skippedDeliveryModelSelection;
            var result = await _fixture.Map();
            Assert.AreEqual(expectShowDeliveryModel, result.ShowDeliveryModelChangeLink);
        }
    }

    public class ConfirmViewModelMapperFixture
    {
        private readonly ConfirmViewModelMapper _sut;

        public ConfirmRequest request { get; }

        public TrainingProgramme trainingProgramme;
        public GetApprenticeshipResponse getApprenticeshipResponse { get; set; }
        public AccountLegalEntityResponse accountLegalEntityResponse { get; set; }
        public GetPriceEpisodesResponse priceEpisodesResponse { get; set; }

        public ChangeEmployerCacheItem cacheItem { get; set; }
        public string encodedAccountLegalEntityId { get; set; }

        public ConfirmViewModelMapperFixture()
        {
            var fixture = new Fixture();
            request = fixture.Create<ConfirmRequest>();
            encodedAccountLegalEntityId = fixture.Create<string>();
            getApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            trainingProgramme = fixture.Create<TrainingProgramme>();
            accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();
            priceEpisodesResponse = new GetPriceEpisodesResponse
            {
                PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                    {
                        new GetPriceEpisodesResponse.PriceEpisode {Cost = 100, FromDate = DateTime.UtcNow}
                    }
            };

            cacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "092023")
                .With(x => x.EndDate, "102024")
                .With(x => x.EmploymentEndDate, "102024")
                .Create();
            var cacheService = new Mock<ICacheStorageService>();
            cacheService.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(cacheItem);

            var commitmentAiClient = new Mock<ICommitmentsApiClient>();

            commitmentAiClient.Setup(x => x.GetApprenticeship(request.ApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(() => getApprenticeshipResponse);
            commitmentAiClient.Setup(x => x.GetAccountLegalEntity(cacheItem.AccountLegalEntityId, It.IsAny<CancellationToken>())).ReturnsAsync(() => accountLegalEntityResponse);
            commitmentAiClient.Setup(x => x.GetPriceEpisodes(request.ApprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(() => priceEpisodesResponse);
            commitmentAiClient
                .Setup(y => y.GetTrainingProgramme(getApprenticeshipResponse.CourseCode, CancellationToken.None))
                .ReturnsAsync(()=> new
                    GetTrainingProgrammeResponse
                    {
                        TrainingProgramme  = trainingProgramme
                    } );

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Encode(It.Is<long>(id => id == cacheItem.AccountLegalEntityId),
                    It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
                .Returns(encodedAccountLegalEntityId);

            _sut = new ConfirmViewModelMapper(commitmentAiClient.Object, Mock.Of<ILogger<ConfirmViewModelMapper>>(), cacheService.Object, encodingService.Object);
        }

        public ConfirmViewModelMapperFixture SetPriceBand(int fundingCap, DateTime startDate)
        {
            trainingProgramme = new TrainingProgramme()
            {
                FundingPeriods = new System.Collections.Generic.List<TrainingProgrammeFundingPeriod>
                {
                    new TrainingProgrammeFundingPeriod
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
            cacheItem.StartDate = startDate.Month.ToString() + startDate.Year.ToString();

            return this;
        }

        public ConfirmViewModelMapperFixture SetPriceInRequest(int price)
        {
            cacheItem.Price = price;
            return this;
        }

        public ConfirmViewModelMapperFixture SetEmploymentEndDateInRequest(DateTime startDate)
        {
            cacheItem.EmploymentEndDate = $"{startDate.Month}{startDate.Year}";
            return this;
        }

        public Task<ConfirmViewModel> Map() => _sut.Map(request);
    }
}
