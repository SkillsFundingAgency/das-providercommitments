using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

[TestFixture]
public class WhenAddingNewPrice
{
    [Test]
    public async Task GetThenCallsPriceViewModelMapper()
    {
        var fixture = new WhenAddingNewPriceFixture();

        await fixture.Sut.Price(fixture.PriceRequest);

        fixture.VerifyPriceViewMapperWasCalled();
    }

    [Test]
    public async Task GetThenReturnsView()
    {
        var fixture = new WhenAddingNewPriceFixture();

        var result = await fixture.Sut.Price(fixture.PriceRequest) as ViewResult;

        result.Should().NotBeNull();
        Assert.That(result.Model.GetType(), Is.EqualTo(typeof(PriceViewModel)));
    }

    [Test]     
    [TestCase(ApprenticeshipStatus.Live)]
    [TestCase(ApprenticeshipStatus.Completed)]
    [TestCase(ApprenticeshipStatus.Paused)]
    [TestCase(ApprenticeshipStatus.WaitingToStart)]
    public async Task PostThenCallsConfirmRequestMapper(ApprenticeshipStatus status)
    {
        var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status } };
        await fixture.Sut.Price(fixture.PriceViewModel);

        fixture.VerifyChangeOfEmployerOverlapAlertRequestWasCalled();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]      
    public async Task PostStoppedRecordThenCallsConfirmRequestMapper(bool isOnChangeOfEmployerPath)
    {
        var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = ApprenticeshipStatus.Stopped } };
        if (isOnChangeOfEmployerPath)
        {
            await fixture.AndStoppedJourneyEligableForChangeOfEmployer().Sut.Price(fixture.PriceViewModel);
        }
        else
        {
            await fixture.AndStoppedJourneyInEligableForChangeOfEmployer().Sut.Price(fixture.PriceViewModel);
        }

        if (isOnChangeOfEmployerPath)
        {
            fixture.VerifyConfirmRequestMapperWasCalled();
        }
        else
        {
            fixture.VerifyChangeOfEmployerOverlapAlertRequestWasCalled();
        }
    }

    [Test]     
    [TestCase(ApprenticeshipStatus.Live)]
    [TestCase(ApprenticeshipStatus.Completed)]
    [TestCase(ApprenticeshipStatus.Paused)]
    [TestCase(ApprenticeshipStatus.WaitingToStart)]
    public async Task PostThenReturnsARedirectResult(ApprenticeshipStatus status)
    {
        var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status } };
        var result = await fixture.Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

        result.Should().NotBeNull();
        Assert.That(result.RouteName, Is.EqualTo(RouteNames.ChangeEmployerOverlapAlert));
    }  
        
    [Test]     
    public async Task PostStoppedRecordThenReturnsARedirectResultToApprenticeConfirmIfEligableForChangeOfEmployer()
    {
        var fixture = new WhenAddingNewPriceFixture { PriceViewModel = {ApprenticeshipStatus = ApprenticeshipStatus.Stopped } };
        var result = await fixture.AndStoppedJourneyEligableForChangeOfEmployer().Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

        result.Should().NotBeNull();
        Assert.That(result.RouteName, Is.EqualTo(RouteNames.ApprenticeConfirm));          
    } 
        
    [Test]
    public async Task PostStoppedRecordThenReturnsARedirectResultToChangeEmployerOverlapAlertIfInEligibleForChangeOfEmployer()
    {
        var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = ApprenticeshipStatus.Stopped } };
        var result = await fixture.AndStoppedJourneyInEligableForChangeOfEmployer().Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

        result.Should().NotBeNull();
        Assert.That(result.RouteName, Is.EqualTo(RouteNames.ChangeEmployerOverlapAlert));
    }
}

public class WhenAddingNewPriceFixture
{
    public ApprenticeController Sut { get; set; }
    public PriceRequest PriceRequest { get; set; }
    public PriceViewModel PriceViewModel { get; set; }
    public ConfirmRequest ConfirmRequest { get; set; }

    private readonly Mock<IModelMapper> _modelMapperMock;
    private readonly Mock<ICommitmentsApiClient> _commitmentsApiMock;
    protected Mock<ICacheStorageService> _cacheStorage;

    private readonly Fixture _fixture;
    private readonly GetApprenticeshipResponse _apprenticeshipResponse;
    private readonly ChangeEmployerCacheItem _changeEmployerCacheItem;

    public WhenAddingNewPriceFixture()
    {
        _fixture = new Fixture();
        PriceRequest = _fixture.Create<PriceRequest>();
        PriceViewModel = _fixture.Create<PriceViewModel>();
        ConfirmRequest = _fixture.Create<ConfirmRequest>();
        _apprenticeshipResponse = _fixture.Create<GetApprenticeshipResponse>();
        _changeEmployerCacheItem = _fixture.Create<ChangeEmployerCacheItem>();
        var fixture = new Fixture();
            
        PriceRequest = fixture.Create<PriceRequest>();
        PriceViewModel = fixture.Create<PriceViewModel>();
        ConfirmRequest = fixture.Create<ConfirmRequest>();

        _modelMapperMock = new Mock<IModelMapper>();
        _modelMapperMock.Setup(x => x.Map<PriceViewModel>(It.IsAny<PriceRequest>()))
            .ReturnsAsync(PriceViewModel);
        _modelMapperMock.Setup(x => x.Map<ConfirmRequest>(It.IsAny<PriceViewModel>()))
            .ReturnsAsync(ConfirmRequest);

        _commitmentsApiMock = new Mock<ICommitmentsApiClient>();
        _commitmentsApiMock.Setup(x => x.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_apprenticeshipResponse);

        _cacheStorage = new Mock<ICacheStorageService>();
        _cacheStorage.Setup(x => x.RetrieveFromCache<ChangeEmployerCacheItem>(It.IsAny<Guid>()))
            .ReturnsAsync(_changeEmployerCacheItem);

        Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<Interfaces.ICookieStorageService<IndexRequest>>(),
            _commitmentsApiMock.Object, Mock.Of<IOuterApiService>(), _cacheStorage.Object);
    }

    public WhenAddingNewPriceFixture AndStoppedJourneyEligableForChangeOfEmployer()
    {
        _changeEmployerCacheItem.StartDate = "092023";
        _apprenticeshipResponse.StopDate = new DateTime(2022, 11, 23);
        return this;
    }

    public WhenAddingNewPriceFixture AndStoppedJourneyInEligableForChangeOfEmployer()
    {
        _changeEmployerCacheItem.StartDate = "092022";
        _apprenticeshipResponse.StopDate = new DateTime(2022, 11, 23);
        return this;
    }

    public void VerifyPriceViewMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<PriceViewModel>(PriceRequest));
    }

    public void VerifyConfirmRequestMapperWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<ConfirmRequest>(PriceViewModel));
    }

    public void VerifyChangeOfEmployerOverlapAlertRequestWasCalled()
    {
        _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerOverlapAlertRequest>(PriceViewModel));
    }
}