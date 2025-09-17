using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.CreateCohortRequestMapperTests;

[TestFixture]
public class WhenIMapCreateCohortRequest
{
    private CreateCohortRequestMapper _mapper;
    private AddDraftApprenticeshipOrRoutePostRequest _source;
    private Mock<ICommitmentsApiClient> _mockCommitmentsApiClient;
    private Mock<ICacheStorageService> _cacheStorageService;
    private long _accountLegalEntityId;
    private Func<Task<CreateCohortRequest>> _act;
    private AccountLegalEntityResponse _accountLegalEntityResponse;
    private CreateCohortCacheItem _cacheItem;


    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _accountLegalEntityId = fixture.Create<long>();
        _accountLegalEntityResponse = fixture.Create<AccountLegalEntityResponse>();

        _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
        _mockCommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(_accountLegalEntityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_accountLegalEntityResponse);

        var birthDate = fixture.Create<DateTime?>();
        var startDate = fixture.Create<DateTime?>();
        var employmentEndDate = fixture.Create<DateTime?>();
        var endDate = fixture.Create<DateTime?>();
        var deliveryModel = fixture.Create<DeliveryModel?>();
        var employmentPrice = fixture.Create<int?>();
        var accountLegalEntityPublicHashedId = fixture.Create<string>();

        _source = fixture.Build<AddDraftApprenticeshipOrRoutePostRequest>()
            .With(x => x.EmployerAccountLegalEntityPublicHashedId, accountLegalEntityPublicHashedId)
            .With(x => x.AccountLegalEntityId, _accountLegalEntityId)
            .With(x => x.BirthDay, birthDate?.Day)
            .With(x => x.BirthMonth, birthDate?.Month)
            .With(x => x.BirthYear, birthDate?.Year)
            .With(x => x.EmploymentEndMonth, employmentEndDate?.Month)
            .With(x => x.EmploymentEndYear, employmentEndDate?.Year)
            .With(x => x.EndMonth, endDate?.Month)
            .With(x => x.EndYear, endDate?.Year)
            .With(x => x.StartMonth, startDate?.Month)
            .With(x => x.StartYear, startDate?.Year)
            .With(x => x.DeliveryModel, deliveryModel)
            .With(x => x.EmploymentPrice, employmentPrice)
            .Without(x => x.StartDate)
            .Without(x => x.Courses)
            .Create();

        _cacheItem = fixture.Build<CreateCohortCacheItem>()
            .With(x => x.AccountLegalEntityId,
                _accountLegalEntityId)
            .Create();
        _cacheStorageService = new Mock<ICacheStorageService>();
        _cacheStorageService.Setup(x => x.RetrieveFromCache<CreateCohortCacheItem>(_source.CacheKey))
            .ReturnsAsync(_cacheItem);

        _mapper = new CreateCohortRequestMapper(_mockCommitmentsApiClient.Object, _cacheStorageService.Object);

        _act = () => _mapper.Map(TestHelper.Clone(_source));
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_cacheItem.ReservationId);
    }

    [Test]
    public async Task ThenFirstNameIsMappedCorrectly()
    {
        var result = await _act();
        result.FirstName.Should().Be(_source.FirstName);
    }

    [Test]
    public async Task ThenEmailIsMappedCorrectly()
    {
        var result = await _act();
        result.Email.Should().Be(_source.Email);
    }

    [Test]
    public async Task ThenDateOfBirthIsMappedCorrectly()
    {
        var result = await _act();
        result.DateOfBirth.Should().Be(_source.DateOfBirth.Date);
    }

    [Test]
    public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
    {
        var result = await _act();
        result.UniqueLearnerNumber.Should().Be(_source.Uln);
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_source.CourseCode);
    }

    [Test]
    public async Task ThenCostIsMappedCorrectly()
    {
        var result = await _act();
        result.Cost.Should().Be(_source.Cost);
    }

    [Test]
    public async Task ThenTrainingPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.TrainingPrice.Should().Be(_source.TrainingPrice);
    }

    [Test]
    public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EndPointAssessmentPrice.Should().Be(_source.EndPointAssessmentPrice);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartDate.Should().Be(_source.StartDate.Date);
    }

    [Test]
    public async Task ThenActualStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.ActualStartDate.Should().Be(_source.ActualStartDate.Date);
    }

    [Test]
    public async Task ThenEmploymentEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentEndDate.Should().Be(_source.EmploymentEndDate.Date);
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EndDate.Should().Be(_source.EndDate.Date);
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.OriginatorReference.Should().Be(_source.Reference);
    }

    [Test]
    public async Task ThenAccountLegalEntityIdIsMappedCorrectly()
    {
        var result = await _act();
        result.AccountLegalEntityId.Should().Be(_accountLegalEntityId);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenAccountIdIsMappedCorrectly()
    {
        var result = await _act();
        result.AccountId.Should().Be(_accountLegalEntityResponse.AccountId);
    }

    [Test]
    public void AndWhenTheAccountLegalEntityIsNotFoundThenShouldThrowInvalidOperationException()
    {
        _mockCommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountLegalEntityResponse) null);

        var action = () => _act();
        action.Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task ThenDeliveryModelIsMappedCorrectly()
    {
        var result = await _act();
        result.DeliveryModel.Should().Be(_source.DeliveryModel);
    }

    [Test]
    public async Task ThenEmploymentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentPrice.Should().Be(_source.EmploymentPrice);
    }

    [Test]
    public async Task ThenLearnerDataIdIsMappedCorrectly()
    {
        var result = await _act();
        result.LearnerDataId.Should().Be(_source.LearnerDataId);
    }
}