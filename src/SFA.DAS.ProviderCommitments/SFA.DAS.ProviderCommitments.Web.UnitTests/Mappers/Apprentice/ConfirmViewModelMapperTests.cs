using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

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

        result.ApprenticeshipHashedId.Should().Be(_fixture.Request.ApprenticeshipHashedId);
    }

    [Test]
    public async Task ThenEmployerAccountLegalEntityPublicHashedIdIsMapped()
    {
        var result = await _fixture.Map();

        result.AccountLegalEntityPublicHashedId.Should().Be(_fixture.EncodedAccountLegalEntityId);
    }

    [Test]
    public async Task ThenOldEmployerNameIsMapped()
    {
        var result = await _fixture.Map();

        result.OldEmployerName.Should().Be(_fixture.GetApprenticeshipResponse.EmployerName);
    }

    [Test]
    public async Task ThenApprenticeNameIsMapped()
    {
        var result = await _fixture.Map();

        result.ApprenticeName.Should().Be($"{_fixture.GetApprenticeshipResponse.FirstName} {_fixture.GetApprenticeshipResponse.LastName}");
    }

    [Test]
    public async Task ThenUlnIsMapped()
    {
        var result = await _fixture.Map();

        result.Uln.Should().Be(_fixture.GetApprenticeshipResponse.Uln);
    }

    [Test]
    public async Task ThenStopDateIsMapped()
    {
        var result = await _fixture.Map();

        result.StopDate.Should().Be(_fixture.GetApprenticeshipResponse.StopDate);
    }

    [Test]
    public async Task ThenOldStartDateIsMapped()
    {
        var result = await _fixture.Map();

        result.OldStartDate.Should().Be(_fixture.GetApprenticeshipResponse.StartDate);
    }

    [Test]
    public async Task ThenOldEndDateIsMapped()
    {
        var result = await _fixture.Map();

        result.OldEndDate.Should().Be(_fixture.GetApprenticeshipResponse.EndDate);
    }


    [Test]
    public async Task ThenOldPriceIsMapped()
    {
        var result = await _fixture.Map();

        ((decimal)result.OldPrice).Should().Be(_fixture.PriceEpisodesResponse.PriceEpisodes.First().Cost);
    }

    [Test]
    public async Task ThenNewEmployerNameIsMapped()
    {
        var result = await _fixture.Map();

        result.NewEmployerName.Should().Be(_fixture.AccountLegalEntityResponse.LegalEntityName);
    }

    [Test]
    public async Task ThenNewStartDateIsMapped()
    {
        var result = await _fixture.Map();
        var expectedStartDate = new MonthYearModel(_fixture.CacheItem.StartDate);

        result.NewStartDate.Should().Be(expectedStartDate.MonthYear);
    }

    [Test]
    public async Task ThenNewEndDateIsMapped()
    {
        var result = await _fixture.Map();
        var expectedEndDate = new MonthYearModel(_fixture.CacheItem.EndDate);

        result.NewEndDate.Should().Be(expectedEndDate.MonthYear);
    }

    [Test]
    public async Task ThenNewEmploymentEndDateIsMapped()
    {
        _fixture.SetEmploymentEndDateInRequest(DateTime.Now);

        var result = await _fixture.Map();

        var expectedEndDate = new MonthYearModel(_fixture.CacheItem.EmploymentEndDate);
        result.NewEmploymentEndDate.Should().Be(expectedEndDate.MonthYear);
    }

    [Test]
    public async Task ThenNewPriceIsMapped()
    {
        var result = await _fixture.Map();

        result.NewPrice.Should().Be(_fixture.CacheItem.Price);
    }

    [Test]
    public async Task ThenNewEmploymentPriceIsMapped()
    {
        var result = await _fixture.Map();

        result.NewEmploymentPrice.Should().Be(_fixture.CacheItem.EmploymentPrice);
    }

    [Test]
    public async Task ThenFundingBandCapIsMapped()
    {
        var priceBand = 1000;
        _fixture.SetStartDateInRequest(DateTime.Now).SetPriceBand(priceBand, DateTime.Now);
        var result = await _fixture.Map();

        result.FundingBandCap.Should().Be(priceBand);
    }

    [TestCase(1000, 900, true)]
    [TestCase(800, 900, false)]
    [TestCase(800, 800, false)]
    public async Task ThenExceedsFundingBandCapIsMapped(int price, int fundingCap, bool fundingCapExceeded)
    {
        _fixture.SetPriceInRequest(price).SetStartDateInRequest(DateTime.Now).SetPriceBand(fundingCap, DateTime.Now);
        var result = await _fixture.Map();

        fundingCapExceeded.Should().Be(result.ExceedsFundingBandCap);
    }

    [Test]
    public async Task Then_ShowDeliveryModel_If_Automatically_Changed_To_Regular()
    {
        _fixture.CacheItem.SkippedDeliveryModelSelection = true;
        _fixture.CacheItem.DeliveryModel = Infrastructure.OuterApi.Types.DeliveryModel.Regular;
        _fixture.GetApprenticeshipResponse.DeliveryModel = DeliveryModel.FlexiJobAgency;
        var result = await _fixture.Map();
        result.ShowDeliveryModel.Should().BeTrue();
    }

    [TestCase(DeliveryModel.Regular, true)]
    [TestCase(DeliveryModel.FlexiJobAgency, true)]
    [TestCase(DeliveryModel.Regular, true)]
    public async Task Then_ShowDeliveryModel_If_Manual_Selection_Was_Made(Infrastructure.OuterApi.Types.DeliveryModel selectedDeliveryModel, bool expectShowDeliveryModel)
    {
        _fixture.CacheItem.SkippedDeliveryModelSelection = false;
        _fixture.CacheItem.DeliveryModel = selectedDeliveryModel;
        var result = await _fixture.Map();
        result.ShowDeliveryModel.Should().Be(expectShowDeliveryModel);
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task Then_ShowDeliveryModelChangeLink_Is_False_If_Selection_Was_Skipped(bool skippedDeliveryModelSelection, bool expectShowDeliveryModel)
    {
        _fixture.CacheItem.SkippedDeliveryModelSelection = skippedDeliveryModelSelection;
        var result = await _fixture.Map();
        result.ShowDeliveryModelChangeLink.Should().Be(expectShowDeliveryModel);
    }
}

public class ConfirmViewModelMapperFixture
{
    private readonly ConfirmViewModelMapper _sut;

    public ConfirmRequest Request { get; }
    public GetApprenticeshipDataResponse GetApprenticeshipDataResponse { get; set; }
    public GetApprenticeshipResponse GetApprenticeshipResponse { get; set; }
    public AccountLegalEntityResponse AccountLegalEntityResponse { get; set; }
    public GetPriceEpisodesResponse PriceEpisodesResponse { get; set; }
    public GetTrainingProgrammeResponse GetTrainingProgrammeResponse { get; set; }
    public TrainingProgramme TrainingProgramme { get; set; }

    public ChangeEmployerCacheItem CacheItem { get; set; }
    public string EncodedAccountLegalEntityId { get; set; }

    public ConfirmViewModelMapperFixture()
    {
        var fixture = new Fixture();
        Request = fixture.Create<ConfirmRequest>();
        EncodedAccountLegalEntityId = fixture.Create<string>();
        GetApprenticeshipResponse = fixture.Create<GetApprenticeshipResponse>();
        GetTrainingProgrammeResponse = fixture.Create<GetTrainingProgrammeResponse>();
        AccountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();
        PriceEpisodesResponse = new GetPriceEpisodesResponse
        {
            PriceEpisodes = new List<GetPriceEpisodesResponse.PriceEpisode>
            {
                new GetPriceEpisodesResponse.PriceEpisode {Cost = 100, FromDate = DateTime.UtcNow}
            }
        };

        GetApprenticeshipDataResponse = fixture.Build<GetApprenticeshipDataResponse>()
            .With(x => x.Apprenticeship, GetApprenticeshipResponse)
            .With(x => x.PriceEpisodes, PriceEpisodesResponse)
            .With(x => x.AccountLegalEntity, AccountLegalEntityResponse)
            .With(x => x.TrainingProgrammeResponse, GetTrainingProgrammeResponse)
            .Create();

        CacheItem = fixture.Build<ChangeEmployerCacheItem>()
            .With(x => x.StartDate, "092023")
            .With(x => x.EndDate, "102024")
            .With(x => x.EmploymentEndDate, "102024")
            .Create();
        var cacheService = new Mock<ICacheStorageService>();
        cacheService.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
            .ReturnsAsync(CacheItem);

        var outerApiClient = new Mock<IOuterApiClient>();

        outerApiClient.Setup(x => x.Get<GetApprenticeshipDataResponse>(It.Is<GetApprenticeshipDataRequest>(r =>
                r.ApprenticeshipId == Request.ApprenticeshipId && r.ProviderId == Request.ProviderId
                                                               && r.AccountLegalEntityId == CacheItem.AccountLegalEntityId)))
            .ReturnsAsync(GetApprenticeshipDataResponse);

        var encodingService = new Mock<IEncodingService>();
        encodingService.Setup(x => x.Encode(It.Is<long>(id => id == CacheItem.AccountLegalEntityId),
                It.Is<EncodingType>(e => e == EncodingType.PublicAccountLegalEntityId)))
            .Returns(EncodedAccountLegalEntityId);

        _sut = new ConfirmViewModelMapper(outerApiClient.Object, Mock.Of<ILogger<ConfirmViewModelMapper>>(), cacheService.Object, encodingService.Object);
    }

    public ConfirmViewModelMapperFixture SetPriceBand(int fundingCap, DateTime startDate)
    {
        TrainingProgramme = new TrainingProgramme()
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

        GetTrainingProgrammeResponse.TrainingProgramme = TrainingProgramme;

        return this;
    }

    public ConfirmViewModelMapperFixture SetStartDateInRequest(DateTime startDate)
    {
        CacheItem.StartDate = startDate.Month.ToString() + startDate.Year.ToString();

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