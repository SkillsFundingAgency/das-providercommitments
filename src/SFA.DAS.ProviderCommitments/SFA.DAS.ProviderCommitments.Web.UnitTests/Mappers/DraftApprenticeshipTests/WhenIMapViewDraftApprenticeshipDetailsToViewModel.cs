using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests;

[TestFixture]
public class WhenIMapViewDraftApprenticeshipDetailsToViewModel
{
    private ViewDraftApprenticeshipViewModelMapper _mapper;
    private DraftApprenticeshipRequest _source;
    private Mock<IOuterApiClient> _outerApiClient;
    private Func<Task<ViewDraftApprenticeshipViewModel>> _act;
    private GetViewDraftApprenticeshipResponse _apiResponse;
    private GetTrainingProgrammeResponse _apiTrainingProgrammeResponse;

    [SetUp]
    public void Arrange()
    {
        var fixture = new Fixture();

        _apiResponse = fixture.Build<GetViewDraftApprenticeshipResponse>().Create();
        _apiTrainingProgrammeResponse = fixture.Build<GetTrainingProgrammeResponse>().Create();

        var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

        commitmentsApiClient.Setup(x => x.GetTrainingProgramme(_apiResponse.CourseCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_apiTrainingProgrammeResponse);

        _outerApiClient = new Mock<IOuterApiClient>();
        _outerApiClient.Setup(x => x.Get<GetViewDraftApprenticeshipResponse>(It.IsAny<GetViewDraftApprenticeshipRequest>()))
            .ReturnsAsync(_apiResponse);

        _mapper = new ViewDraftApprenticeshipViewModelMapper(commitmentsApiClient.Object, _outerApiClient.Object);
        _source = fixture.Build<DraftApprenticeshipRequest>().Create();

        _act = async () => await _mapper.Map(TestHelper.Clone(_source));
    }

    [Test]
    public async Task ThenProviderIdIsMappedCorrectly()
    {
        var result = await _act();
        result.ProviderId.Should().Be(_source.ProviderId);
    }

    [Test]
    public async Task ThenCohortReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.CohortReference.Should().Be(_source.CohortReference);
    }

    [Test]
    public async Task ThenFirstNameIsMappedCorrectly()
    {
        var result = await _act();
        result.FirstName.Should().Be(_apiResponse.FirstName);
    }

    [Test]
    public async Task ThenLastNameIsMappedCorrectly()
    {
        var result = await _act();
        result.LastName.Should().Be(_apiResponse.LastName);
    }

    [Test]
    public async Task ThenEmailIsMappedCorrectly()
    {
        var result = await _act();
        result.Email.Should().Be(_apiResponse.Email);
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
        result.DateOfBirth.Should().Be(_apiResponse.DateOfBirth);
    }

    [TestCase(DeliveryModel.Regular)]
    [TestCase(DeliveryModel.PortableFlexiJob)]
    public async Task ThenDeliveryModelIsMappedCorrectly(DeliveryModel dm)
    {
        _apiResponse.DeliveryModel = dm;
        var result = await _act();
        result.DeliveryModel.Should().Be(dm);
    }

    [Test]
    public async Task ThenCourseNameIsMappedCorrectly()
    {
        var result = await _act();
        result.TrainingCourse.Should().Be(_apiTrainingProgrammeResponse.TrainingProgramme.Name);
    }

    [Test]
    public async Task ThenEmploymentPriceIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentPrice.Should().Be(_apiResponse.EmploymentPrice);
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
    public async Task ThenCostIsMappedCorrectly()
    {
        var result = await _act();
        result.Cost.Should().Be(_apiResponse.Cost);
    }

    [Test]
    public async Task ThenStartDateIsMappedCorrectly()
    {
        var result = await _act();
        result.StartDate.Should().Be(_apiResponse.StartDate);
    }

    [Test]
    public async Task ThenEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EndDate.Should().Be(_apiResponse.EndDate);
    }

    [Test]
    public async Task ThenEmploymentEndDateIsMappedCorrectly()
    {
        var result = await _act();
        result.EmploymentEndDate.Should().Be(_apiResponse.EmploymentEndDate);
    }

    [Test]
    public async Task ThenOriginatorReferenceIsMappedCorrectly()
    {
        var result = await _act();
        result.Reference.Should().Be(_apiResponse.Reference);
    }
        
    [Test]
    public async Task ThenTheVersionIsMapped()
    {
        var result = await _act();
        result.TrainingCourseVersion.Should().Be(_apiResponse.TrainingCourseVersion);
    }
                
    [Test]
    public async Task ThenTheSelectedOptionIsMapped()
    {
        var result = await _act();
        result.TrainingCourseOption.Should().Be(_apiResponse.TrainingCourseOption);
    }
        
    [Test]
    public async Task ThenTheSelectedOptionIsDisplayedAsToBeConfirmedIfEmpty()
    {
        _apiResponse.TrainingCourseOption = string.Empty;
        var result = await _act();
        result.TrainingCourseOption.Should().Be("To be confirmed");
    }
        
    [Test]
    public async Task ThenTheSelectedOptionIsDisplayedAsEmptyIfNullFromApi()
    {
        _apiResponse.TrainingCourseOption = null;
        var result = await _act();
        result.TrainingCourseOption.Should().Be("");
    }
        
    [TestCase(true)]
    [TestCase(false)]
    public async Task ThenHasOptionsIsMappedCorrectly(bool hasOptions)
    {
        _apiResponse.HasStandardOptions = hasOptions;
        var result = await _act();
        result.HasTrainingCourseOption.Should().Be(hasOptions);
    }

    [Test]
    public async Task ThenRecognisePriorLearningIsMappedCorrectly()
    {
        var result = await _act();
        result.RecognisePriorLearning.Should().Be(_apiResponse.RecognisePriorLearning);
    }

    [Test]
    public async Task ThenRecognisingPriorLearningStillNeedsToBeConsideredIsMappedCorrectly()
    {
        var result = await _act();
        result.RecognisingPriorLearningStillNeedsToBeConsidered.Should().Be(_apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered);
    }

    [Test]
    public async Task ThenRecognisingPriorLearningExtendedStillNeedsToBeConsideredIsMappedCorrectly()
    {
        var result = await _act();
        result.RecognisingPriorLearningExtendedStillNeedsToBeConsidered.Should().Be(_apiResponse.RecognisingPriorLearningExtendedStillNeedsToBeConsidered);
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
    public async Task ThenDurationReducedByHoursIsMappedCorrectly()
    {
        var result = await _act();
        result.DurationReducedByHours.Should().Be(_apiResponse.DurationReducedByHours);
    }

    [Test]
    public async Task ThenIsDurationReducedByRplIsMappedCorrectly()
    {
        var result = await _act();
        result.IsDurationReducedByRpl.Should().Be(_apiResponse.IsDurationReducedByRpl);
    }

    [Test]
    public async Task ThenTrainingTotalHoursIsMappedCorrectly()
    {
        var result = await _act();
        result.TrainingTotalHours.Should().Be(_apiResponse.TrainingTotalHours);
    }
}