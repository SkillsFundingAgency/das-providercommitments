using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    [TestFixture]
    public class ChangeOfEmployerOverlapAlertViewModelMapperTests
    {
        private ChangeOfEmployerOverlapAlertViewModelMapperFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new ChangeOfEmployerOverlapAlertViewModelMapperFixture();
        }

        [Test]
        public async Task ThenApprenticeshipHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.ApprenticeshipHashedId, Is.EqualTo(_fixture.request.ApprenticeshipHashedId));
        }

        [Test]
        public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.AccountLegalEntityPublicHashedId, Is.EqualTo(_fixture.encodedAccountLegalEntityId));
        }

        [Test]
        public async Task ThenOldEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.OldEmployerName, Is.EqualTo(_fixture.getApprenticeshipResponse.EmployerName));
        }

        [Test]
        public async Task ThenApprenticeNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(
                result.ApprenticeName, Is.EqualTo($"{_fixture.getApprenticeshipResponse.FirstName} {_fixture.getApprenticeshipResponse.LastName}"));
        }


        [Test]
        public async Task ThenStopDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.StopDate, Is.EqualTo(_fixture.getApprenticeshipResponse.StopDate));
        }

        [Test]
        public async Task ThenOldStartDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.OldStartDate, Is.EqualTo(_fixture.getApprenticeshipResponse.StartDate));
        }

        [Test]
        public async Task ThenOldEndDateIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.OldEndDate, Is.EqualTo(_fixture.getApprenticeshipResponse.EndDate));
        }


        [Test]
        public async Task ThenOldPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.OldPrice, Is.EqualTo(_fixture.priceEpisodesResponse.PriceEpisodes.First().Cost));
        }

        [Test]
        public async Task ThenNewEmployerNameIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.NewEmployerName, Is.EqualTo(_fixture.accountLegalEntityResponse.LegalEntityName));
        }

        [Test]
        public async Task ThenNewStartDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedStartDate = new MonthYearModel(_fixture.cacheItem.StartDate);

            Assert.That(result.NewStartDate, Is.EqualTo(expectedStartDate.MonthYear));
        }

        [Test]
        public async Task ThenNewEndDateIsMapped()
        {
            var result = await _fixture.Map();
            var expectedEndDate = new MonthYearModel(_fixture.cacheItem.EndDate);

            Assert.That(result.NewEndDate, Is.EqualTo(expectedEndDate.MonthYear));
        }

        [Test]
        public async Task ThenNewEmploymentEndDateIsMapped()
        {
            _fixture.SetEmploymentEndDateInRequest(DateTime.Now);

            var result = await _fixture.Map();

            var expectedEndDate = new MonthYearModel(_fixture.cacheItem.EmploymentEndDate);
            Assert.That(result.NewEmploymentEndDate, Is.EqualTo(expectedEndDate.MonthYear));
        }

        [Test]
        public async Task ThenNewPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.NewPrice, Is.EqualTo(_fixture.cacheItem.Price));
        }

        [Test]
        public async Task ThenNewEmploymentPriceIsMapped()
        {
            var result = await _fixture.Map();

            Assert.That(result.NewEmploymentPrice, Is.EqualTo(_fixture.cacheItem.EmploymentPrice));
        }

        [Test]
        public async Task ThenFundingBandCapIsMapped()
        {
            var priceBand = 1000;
            _fixture.SetStartDateInRequest(DateTime.Now).SetPriceBand(priceBand, DateTime.Now);
            var result = await _fixture.Map();

            Assert.That(result.FundingBandCap, Is.EqualTo(priceBand));
        }

        [TestCase(1000, 900, true)]
        [TestCase(800, 900, false)]
        [TestCase(800, 800, false)]
        public async Task ThenExceedsFundingBandCapIsMapped(int price, int fundingCap, bool fundingCapExceeded)
        {
            _fixture.SetPriceInRequest(price).SetStartDateInRequest(DateTime.Now)
                .SetPriceBand(fundingCap, DateTime.Now);
            var result = await _fixture.Map();

            Assert.That(fundingCapExceeded, Is.EqualTo(result.ExceedsFundingBandCap));
        }

        [Test]
        public async Task Then_ShowDeliveryModel_If_Automatically_Changed_To_Regular()
        {
            _fixture.cacheItem.SkippedDeliveryModelSelection = true;
            _fixture.cacheItem.DeliveryModel = Infrastructure.OuterApi.Types.DeliveryModel.Regular;
            _fixture.getApprenticeshipResponse.DeliveryModel = DeliveryModel.FlexiJobAgency;
            var result = await _fixture.Map();
            Assert.IsTrue(result.ShowDeliveryModel);
        }

        [TestCase(DeliveryModel.Regular, true)]
        [TestCase(DeliveryModel.FlexiJobAgency, true)]
        [TestCase(DeliveryModel.Regular, true)]
        public async Task Then_ShowDeliveryModel_If_Manual_Selection_Was_Made(
            Infrastructure.OuterApi.Types.DeliveryModel selectedDeliveryModel, bool expectShowDeliveryModel)
        {
            _fixture.cacheItem.SkippedDeliveryModelSelection = false;
            _fixture.cacheItem.DeliveryModel = selectedDeliveryModel;
            var result = await _fixture.Map();
            Assert.That(result.ShowDeliveryModel, Is.EqualTo(expectShowDeliveryModel));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Then_ShowDeliveryModelChangeLink_Is_False_If_Selection_Was_Skipped(
            bool skippedDeliveryModelSelection, bool expectShowDeliveryModel)
        {
            _fixture.cacheItem.SkippedDeliveryModelSelection = skippedDeliveryModelSelection;
            var result = await _fixture.Map();
            Assert.That(result.ShowDeliveryModelChangeLink, Is.EqualTo(expectShowDeliveryModel));
        }
    }

    public class ChangeOfEmployerOverlapAlertViewModelMapperFixture
    {
        private readonly ChangeOfEmployerOverlapAlertViewModelMapper _sut;

        public ChangeOfEmployerOverlapAlertRequest request { get; }
        public GetApprenticeshipDataResponse getApprenticeshipDataResponse { get; set; }
        public GetApprenticeshipResponse getApprenticeshipResponse { get; set; }
        public AccountLegalEntityResponse accountLegalEntityResponse { get; set; }
        public GetPriceEpisodesResponse priceEpisodesResponse { get; set; }
        public GetTrainingProgrammeResponse getTrainingProgrammeResponse { get; set; }
        public TrainingProgramme trainingProgramme { get; set; }

        public ChangeEmployerCacheItem cacheItem { get; set; }
        public string encodedAccountLegalEntityId { get; set; }

        public ChangeOfEmployerOverlapAlertViewModelMapperFixture()
        {
            var fixture = new Fixture();
            request = fixture.Create<ChangeOfEmployerOverlapAlertRequest>();
            encodedAccountLegalEntityId = fixture.Create<string>();
            getApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
            getTrainingProgrammeResponse = fixture.Create<GetTrainingProgrammeResponse>();
            accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();
            priceEpisodesResponse = new GetPriceEpisodesResponse
            {
                PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
                {
                    new GetPriceEpisodesResponse.PriceEpisode { Cost = 100, FromDate = DateTime.UtcNow }
                }
            };

            getApprenticeshipDataResponse = fixture.Build<GetApprenticeshipDataResponse>()
               .With(x => x.Apprenticeship, getApprenticeshipResponse)
               .With(x => x.PriceEpisodes, priceEpisodesResponse)
               .With(x => x.AccountLegalEntity, accountLegalEntityResponse)
               .With(x => x.TrainingProgrammeResponse, getTrainingProgrammeResponse)
               .Create();

            cacheItem = fixture.Build<ChangeEmployerCacheItem>()
                .With(x => x.StartDate, "092023")
                .With(x => x.EndDate, "102024")
                .With(x => x.EmploymentEndDate, "102024")
                .Create();
            var cacheService = new Mock<ICacheStorageService>();
            cacheService.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
                .ReturnsAsync(cacheItem);

            var outerApiClient = new Mock<IOuterApiClient>();

            outerApiClient.Setup(x => x.Get<GetApprenticeshipDataResponse>(It.Is<GetApprenticeshipDataRequest>(r =>
                  r.ApprenticeshipId == request.ApprenticeshipId && r.ProviderId == request.ProviderId
                  && r.AccountLegalEntityId == cacheItem.AccountLegalEntityId)))
              .ReturnsAsync(getApprenticeshipDataResponse);

            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Encode(It.Is<long>(id => id == cacheItem.AccountLegalEntityId),
                    It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
                .Returns(encodedAccountLegalEntityId);

            _sut = new ChangeOfEmployerOverlapAlertViewModelMapper(outerApiClient.Object,
                Mock.Of<ILogger<ChangeOfEmployerOverlapAlertViewModelMapper>>(), cacheService.Object,
                encodingService.Object);
        }

        public ChangeOfEmployerOverlapAlertViewModelMapperFixture SetPriceBand(int fundingCap, DateTime startDate)
        {
            trainingProgramme = new TrainingProgramme()
            {
                FundingPeriods = new List<TrainingProgrammeFundingPeriod>
                {
                    new TrainingProgrammeFundingPeriod
                    {
                        EffectiveFrom = startDate.AddDays(-1),
                        EffectiveTo = startDate.AddDays(1),
                        FundingCap = fundingCap
                    }
                }
            };

            getTrainingProgrammeResponse.TrainingProgramme = trainingProgramme;

            return this;
        }

        public ChangeOfEmployerOverlapAlertViewModelMapperFixture SetStartDateInRequest(DateTime startDate)
        {
            cacheItem.StartDate = startDate.Month.ToString() + startDate.Year.ToString();

            return this;
        }

        public ChangeOfEmployerOverlapAlertViewModelMapperFixture SetPriceInRequest(int price)
        {
            cacheItem.Price = price;
            return this;
        }

        public ChangeOfEmployerOverlapAlertViewModelMapperFixture SetEmploymentEndDateInRequest(DateTime startDate)
        {
            cacheItem.EmploymentEndDate = $"{startDate.Month}{startDate.Year}";
            return this;
        }

        public Task<ChangeOfEmployerOverlapAlertViewModel> Map() => _sut.Map(request);
    }
}