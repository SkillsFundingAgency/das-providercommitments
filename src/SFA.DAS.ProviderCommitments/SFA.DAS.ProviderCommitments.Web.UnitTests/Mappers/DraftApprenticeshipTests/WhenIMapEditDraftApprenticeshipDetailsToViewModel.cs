using System;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests;

[TestFixture]
public class WhenIMapEditDraftApprenticeshipDetailsToViewModel
{
    private EditDraftApprenticeshipViewModelMapper _mapper;
    private EditDraftApprenticeshipRequest _source;
    private Func<Task<EditDraftApprenticeshipViewModel>> _act;
    private GetEditDraftApprenticeshipResponse _apiResponse;
    private Mock<ITempDataStorageService> _tempDataStorageService;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _apiResponse = fixture.Build<GetEditDraftApprenticeshipResponse>().Create();
        var commitmentsApiClient = new Mock<IOuterApiClient>();
        commitmentsApiClient.Setup(x =>
                x.Get<GetEditDraftApprenticeshipResponse>(It.IsAny<GetEditDraftApprenticeshipRequest>()))
            .ReturnsAsync(_apiResponse);

        _tempDataStorageService = new Mock<ITempDataStorageService>();
        _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditDraftApprenticeshipViewModel>())
            .Returns(() => null);

        _mapper = new EditDraftApprenticeshipViewModelMapper(Mock.Of<IEncodingService>(), commitmentsApiClient.Object, _tempDataStorageService.Object);
        _source = fixture.Build<EditDraftApprenticeshipRequest>().Create();

        _act = async () => (await _mapper.Map(TestHelper.Clone(_source))) as EditDraftApprenticeshipViewModel;
    }

    [Test]
    public async Task ThenDraftApprenticeshipIdIsMappedCorrectly()
    {
        var result = await _act();
        result.DraftApprenticeshipId.Should().Be(_source.Request.DraftApprenticeshipId);
    }

    [Test]
    public async Task ThenCohortIdIsMappedCorrectly()
    {
        var result = await _act();
        result.CohortId.Should().Be(_source.Request.CohortId);
    }

    [Test]
    public async Task ThenCohortReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.CohortReference.Should().Be(_source.Request.CohortReference);
    }

    [Test]
    public async Task ThenReservationIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ReservationId.Should().Be(_apiResponse.ReservationId);
    }

    [Test]
    public async Task ThenFirstNameIsMappedCorrectly()
    {
        var result = await _act();
        result.FirstName.Should().Be(_apiResponse.FirstName);
    }

    [Test]
    public async Task ThenEmailIsMappedCorrectly()
    {
        var result = await _act();
        result.Email.Should().Be(_apiResponse.Email);
    }

    [Test]
    public async Task ThenEmailAddressConfirmedIsMappedCorrectly()
    {
        var result = await _act();
        result.EmailAddressConfirmed.Should().Be(_apiResponse.EmailAddressConfirmed);
    }


    [Test]
    public async Task ThenLastNameIsMappedCorrectly()
    {
        var result = await _act();
        result.LastName.Should().Be(_apiResponse.LastName);
    }

    [Test]
    public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
    {
        var result = await _act();
        result.Uln.Should().Be(_apiResponse.Uln);
    }

    [Test]
    public async Task ThenDateOfBirthIsMappedCorrectly()
    {
        var result = await _act();
        using (new AssertionScope())
        {
            result.DateOfBirth.Day.Should().Be(_apiResponse.DateOfBirth?.Day);
            result.DateOfBirth.Month.Should().Be(_apiResponse.DateOfBirth?.Month);
            result.DateOfBirth.Year.Should().Be(_apiResponse.DateOfBirth?.Year);
        }
    }

    [Test]
    public async Task ThenCourseCodeIsMappedCorrectly()
    {
        var result = await _act();
        result.CourseCode.Should().Be(_apiResponse.CourseCode);
    }

    [Test]
    public async Task ThenCostIsMappedCorrectly()
    {
        var result = await _act();
        result.Cost.Should().Be(_apiResponse.Cost);
    }

    [Test]
    public async Task ThenTrainingPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.TrainingPrice.Should().Be(_apiResponse.TrainingPrice);
    }

    [Test]
    public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EndPointAssessmentPrice.Should().Be(_apiResponse.EndPointAssessmentPrice);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        using (new AssertionScope())
        {
            result.StartDate.Month.Should().Be(_apiResponse.StartDate?.Month);
            result.StartDate.Year.Should().Be(_apiResponse.StartDate?.Year);
        }
    }

    [Test]
    public async Task ThenActualStartDateIsMappedCorrectly()
    {
        var result = await _act();
        using (new AssertionScope())
        {
            result.ActualStartDate.Month.Should().Be(_apiResponse.ActualStartDate?.Month);
            result.ActualStartDate.Day.Should().Be(_apiResponse.ActualStartDate?.Day);
            result.ActualStartDate.Year.Should().Be(_apiResponse.ActualStartDate?.Year);
        }
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _act();
        using (new AssertionScope())
        {
            result.EndDate.Month.Should().Be(_apiResponse.EndDate?.Month);
            result.EndDate.Year.Should().Be(_apiResponse.EndDate?.Year);
        }
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.Reference.Should().Be(_apiResponse.ProviderReference);
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.Request.ProviderId);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenTheTrainingCourseOptionIsMapped(bool hasStandardOptions)
    {
        _apiResponse.HasStandardOptions = hasStandardOptions;

        var result = await _act();

        result.HasStandardOptions.Should().Be(_apiResponse.HasStandardOptions);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenIsContinuationIsMappedCorrectly(bool isContinuation)
    {
        _apiResponse.IsContinuation = isContinuation;
        var result = await _act();
        result.IsContinuation.Should().Be(_apiResponse.IsContinuation);
    }

    [Test]
    public async Task ThenTheTrainingCourseOptionIsMapped()
    {
        var result = await _act();
        result.TrainingCourseOption.Should().Be(_apiResponse.TrainingCourseOption);
    }
        
    [Test]
    public async Task ThenTheTrainingCourseOptionIsMappedToMinusOneIfEmpty()
    {
        _apiResponse.TrainingCourseOption = string.Empty;
        var result = await _act();
        result.TrainingCourseOption.Should().Be("-1");
    }

    [TestCase(DeliveryModel.Regular)]
    [TestCase(DeliveryModel.PortableFlexiJob)]
    public async Task ThenDeliveryModelIsMappedCorrectly(DeliveryModel dm)
    {
        _apiResponse.DeliveryModel = (Infrastructure.OuterApi.Types.DeliveryModel) dm;
        var result = await _act();
        result.DeliveryModel.Should().Be(dm);
    }

    [Test]
    public async Task ThenEmploymentPriceIsMappedCorrectly()
    {
        _apiResponse.EmploymentPrice = 1234;
        var result = await _act();
        result.EmploymentPrice.Should().Be(1234);
    }

    [Test]
    public async Task ThenEmploymentEndDateIsMappedCorrectly()
    {
        var result = await _act();
        using (new AssertionScope())
        {
            result.EmploymentEndDate.Month.Should().Be(_apiResponse.EmploymentEndDate?.Month);
            result.EmploymentEndDate.Year.Should().Be(_apiResponse.EmploymentEndDate?.Year);
        }
    }

    [Test]
    public async Task ThenRecognisePriorLearningIsMappedCorrectly()
    {
        var result = await _act();
        result.RecognisePriorLearning.Should().Be(_apiResponse.RecognisePriorLearning);
    }

    [Test]
    public async Task ThenDurationReducedByIsMappedCorrectly()
    {
        var result = await _act();
        result.DurationReducedBy.Should().Be(_apiResponse.DurationReducedBy);
    }

    [Test]
    public async Task ThenPriceReducedByIsMappedCorrectly()
    {
        var result = await _act();
        result.PriceReducedBy.Should().Be(_apiResponse.PriceReducedBy);
    }

    [Test]
    public async Task ThenRecognisingPriorLearningStillNeedsToBeConsideredIsMappedCorrectly()
    {
        var result = await _act();
        result.RecognisingPriorLearningStillNeedsToBeConsidered.Should().Be(_apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered);
    }

    [TestCase(true, Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency, true)]
    [TestCase(true, Infrastructure.OuterApi.Types.DeliveryModel.PortableFlexiJob, false)]
    [TestCase(true, Infrastructure.OuterApi.Types.DeliveryModel.Regular, false)]
    [TestCase(false, Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency, false)]
    [TestCase(false, Infrastructure.OuterApi.Types.DeliveryModel.PortableFlexiJob, false)]
    [TestCase(false, Infrastructure.OuterApi.Types.DeliveryModel.Regular, false)]

    public async Task ThenHasUnavailableFlexiJobAgencyDeliveryModelIsMappedCorrectly(
        bool hasUnavailableDeliveryModel, Infrastructure.OuterApi.Types.DeliveryModel deliveryModel,
        bool expectedResult)
    {
        _apiResponse.DeliveryModel = deliveryModel;
        _apiResponse.HasUnavailableDeliveryModel = hasUnavailableDeliveryModel;

        var result = await _act();
        result.HasUnavailableFlexiJobAgencyDeliveryModel.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenEmployerHasEditedCostIsMappedCorrectly()
    {
        var result = await _act();
        result.EmployerHasEditedCost.Should().Be(_apiResponse.EmployerHasEditedCost);
    }

    [Test]
    public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
    {
        var result = await _act();
        result.HasMultipleDeliveryModelOptions.Should().Be(_apiResponse.HasMultipleDeliveryModelOptions);
    }

    [TestCase(DeliveryModel.Regular, Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency, true)]
    [TestCase(DeliveryModel.FlexiJobAgency, Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency, false)]
    public async Task ThenHasChangedDeliveryModelIsMappedCorrectly(DeliveryModel editedDeliveryModel, Infrastructure.OuterApi.Types.DeliveryModel persistedDeliveryModel, bool expectedResult)
    {
        _tempDataStorageService.Setup(x => x.RetrieveFromCache<EditDraftApprenticeshipViewModel>())
            .Returns(new EditDraftApprenticeshipViewModel(null, null, null, null)
            {
                DeliveryModel = editedDeliveryModel
            });

        _apiResponse.DeliveryModel = persistedDeliveryModel;

        var result = await _act();
        result.HasChangedDeliveryModel.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenLearnerDataIdIsMappedCorrectly()
    {
        var result = await _act();
        result.LearnerDataId.Should().Be(_apiResponse.LearnerDataId);
    }
}