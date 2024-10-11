using System;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class AddDraftApprenticeshipRedirectModelMapperTests
{
    private AddDraftApprenticeshipRedirectModelMapper _mapper;
    private AddDraftApprenticeshipOrRoutePostRequest _source;
    private Func<Task<AddDraftApprenticeshipRedirectModel>> _act;
    private Mock<ICacheStorageService> _cacheStorageService;
    private CreateCohortCacheItem _cacheItem;
    private Mock<IOuterApiService> _apiService;
    private ValidateUlnOverlapOnStartDateQueryResult _validateUlnOverlapOnStartDateResult;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();
        _source = fixture.Build<AddDraftApprenticeshipOrRoutePostRequest>().Without(x => x.BirthDay).Without(x => x.BirthMonth).Without(x => x.BirthYear)
            .Without(x => x.StartMonth).Without(x => x.StartYear).Without(x => x.StartDate)
            .Without(x => x.EndMonth).Without(x => x.EndYear)
            .With(x => x.ChangeDeliveryModel, string.Empty)
            .With(x => x.ChangeCourse, string.Empty)
            .With(x => x.ChangePilotStatus, string.Empty)
            .Create();
        _source.StartDate = new MonthYearModel("092022");

        _cacheItem = fixture.Create<CreateCohortCacheItem>();
        _cacheStorageService = new Mock<ICacheStorageService>();
        _cacheStorageService
            .Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(It.Is<Guid>(key => key == _source.CacheKey)))
            .ReturnsAsync(_cacheItem);

        _apiService = new Mock<IOuterApiService>();

        _validateUlnOverlapOnStartDateResult = new ValidateUlnOverlapOnStartDateQueryResult
        {
            HasOverlapWithApprenticeshipId = null,
            HasStartDateOverlap = false
        };
        _apiService.Setup(x => x.ValidateUlnOverlapOnStartDate(It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(() => _validateUlnOverlapOnStartDateResult);

        _mapper = new AddDraftApprenticeshipRedirectModelMapper(_cacheStorageService.Object, _apiService.Object);

        _act = async () => await _mapper.Map(_source);
    }

    [Test]
    public async Task Then_CacheKey_Is_Mapped_Correctly()
    {
        var result = await _act();
        result.CacheKey.Should().Be(_source.CacheKey);
    }

    [Test]
    public async Task Then_ProviderId_Is_Mapped_Correctly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task Then_FirstName_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.FirstName == _source.FirstName),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_LastName_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.LastName == _source.LastName),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_Uln_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.Uln == _source.Uln),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_Email_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.Email == _source.Email),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_DateOfBirth_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.DateOfBirth == _source.DateOfBirth.Date),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_StartDate_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.StartDate == _source.StartDate.Date),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_EndDate_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.EndDate == _source.EndDate.Date),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_ActualStartDate_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.ActualStartDate == _source.ActualStartDate.Date),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_EmploymentEndDate_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.EmploymentEndDate == _source.EmploymentEndDate.Date),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_EmploymentPrice_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.EmploymentPrice == _source.EmploymentPrice),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_Cost_Is_Saved_To_Cache()
    {
        _source.IsOnFlexiPaymentPilot = false;
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.Cost == _source.Cost),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_TrainingPrice_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.TrainingPrice == _source.TrainingPrice),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_EndPointAssessmentPrice_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.EndPointAssessmentPrice == _source.EndPointAssessmentPrice),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_Calculated_Cost_Is_Saved_To_Cache_For_Pilot_Providers()
    {
        _source.IsOnFlexiPaymentPilot = true;
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.Cost == _source.TrainingPrice + _source.EndPointAssessmentPrice),
            It.IsAny<int>()));
    }

    [Test]
    public async Task Then_Reference_Is_Saved_To_Cache()
    {
        var result = await _act();
        _cacheStorageService.Verify(x => x.SaveToCache(It.IsAny<Guid>(),
            It.Is<CreateCohortCacheItem>(y => y.Reference == _source.Reference),
            It.IsAny<int>()));
    }

    [Test]
    public async Task When_User_Opts_To_Change_Course_Then_RedirectTo_Is_SelectCourse()
    {
        _source.ChangeCourse = "Edit";
        var result = await _act();
        result.RedirectTo.Should().Be(AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectCourse);
    }

    [Test]
    public async Task When_User_Opts_To_ChangeDeliveryModel_Then_RedirectTo_Is_SelectDeliveryModel()
    {
        _source.ChangeDeliveryModel = "Edit";
        var result = await _act();
        result.RedirectTo.Should().Be(AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectDeliveryModel);
    }

    [Test]
    public async Task When_User_Opts_To_Change_FlexiPayments_Pilot_Status_Then_RedirectTo_Is_SelectPilotStatus()
    {
        _source.ChangePilotStatus = "Edit";
        var result = await _act();
        result.RedirectTo.Should().Be(AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectPilotStatus);
    }

    [Test]
    public async Task When_There_Is_An_Overlap_Then_RedirectTo_Is_OverlapWarning()
    {
        _source.EndDay = 1;
        _source.EndMonth = DateTime.UtcNow.Month;
        _source.EndYear = DateTime.UtcNow.Year + 2;

        _validateUlnOverlapOnStartDateResult.HasOverlapWithApprenticeshipId = 123;
        _validateUlnOverlapOnStartDateResult.HasStartDateOverlap = true;

        var result = await _act();

        result.RedirectTo.Should().Be(AddDraftApprenticeshipRedirectModel.RedirectTarget.OverlapWarning);
    }

    [Test]
    public async Task When_There_Is_An_Overlap_Then_OverlappingApprenticeshipId_Is_Mapped_Correctly()
    {
        _source.EndDay = 1;
        _source.EndMonth = DateTime.UtcNow.Month;
        _source.EndYear = DateTime.UtcNow.Year + 2;

        _validateUlnOverlapOnStartDateResult.HasOverlapWithApprenticeshipId = 123;
        _validateUlnOverlapOnStartDateResult.HasStartDateOverlap = true;

        var result = await _act();

        result.OverlappingApprenticeshipId.Should().Be(123);
    }
}