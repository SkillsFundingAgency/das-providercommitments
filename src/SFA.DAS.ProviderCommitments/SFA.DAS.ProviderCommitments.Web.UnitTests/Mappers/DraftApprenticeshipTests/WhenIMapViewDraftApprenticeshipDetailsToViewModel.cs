using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
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
            Assert.That(result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CohortReference, Is.EqualTo(_source.CohortReference));
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.FirstName, Is.EqualTo(_apiResponse.FirstName));
        }

        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.LastName, Is.EqualTo(_apiResponse.LastName));
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Email, Is.EqualTo(_apiResponse.Email));
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Uln, Is.EqualTo(_apiResponse.Uln));
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DateOfBirth, Is.EqualTo(_apiResponse.DateOfBirth));
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task ThenDeliveryModelIsMappedCorrectly(DeliveryModel dm)
        {
            _apiResponse.DeliveryModel = dm;
            var result = await _act();
            Assert.That(result.DeliveryModel, Is.EqualTo(dm));
        }

        [Test]
        public async Task ThenCourseNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.TrainingCourse, Is.EqualTo(_apiTrainingProgrammeResponse.TrainingProgramme.Name));
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentPrice, Is.EqualTo(_apiResponse.EmploymentPrice));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_apiResponse.Cost));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.StartDate, Is.EqualTo(_apiResponse.StartDate));
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndDate, Is.EqualTo(_apiResponse.EndDate));
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmploymentEndDate, Is.EqualTo(_apiResponse.EmploymentEndDate));
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Reference, Is.EqualTo(_apiResponse.Reference));
        }
        
        [Test]
        public async Task ThenTheVersionIsMapped()
        {
            var result = await _act();
            Assert.That(result.TrainingCourseVersion, Is.EqualTo(_apiResponse.TrainingCourseVersion));
        }
                
        [Test]
        public async Task ThenTheSelectedOptionIsMapped()
        {
            var result = await _act();
            Assert.That(result.TrainingCourseOption, Is.EqualTo(_apiResponse.TrainingCourseOption));
        }
        
        [Test]
        public async Task ThenTheSelectedOptionIsDisplayedAsToBeConfirmedIfEmpty()
        {
            _apiResponse.TrainingCourseOption = string.Empty;
            var result = await _act();
            Assert.That(result.TrainingCourseOption, Is.EqualTo("To be confirmed"));
        }
        
        [Test]
        public async Task ThenTheSelectedOptionIsDisplayedAsEmptyIfNullFromApi()
        {
            _apiResponse.TrainingCourseOption = null;
            var result = await _act();
            Assert.That(result.TrainingCourseOption, Is.EqualTo(""));
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenHasOptionsIsMappedCorrectly(bool hasOptions)
        {
            _apiResponse.HasStandardOptions = hasOptions;
            var result = await _act();
            Assert.That(result.HasTrainingCourseOption, Is.EqualTo(hasOptions));
        }

        [Test]
        public async Task ThenRecognisePriorLearningIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.RecognisePriorLearning, Is.EqualTo(_apiResponse.RecognisePriorLearning));
        }

        [Test]
        public async Task ThenRecognisingPriorLearningStillNeedsToBeConsideredIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.RecognisingPriorLearningStillNeedsToBeConsidered, Is.EqualTo(_apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered));
        }

        [Test]
        public async Task ThenRecognisingPriorLearningExtendedStillNeedsToBeConsideredIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.RecognisingPriorLearningExtendedStillNeedsToBeConsidered, Is.EqualTo(_apiResponse.RecognisingPriorLearningExtendedStillNeedsToBeConsidered));
        }

        [Test]
        public async Task ThenDurationReducedByIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DurationReducedBy, Is.EqualTo(_apiResponse.DurationReducedBy));
        }

        [Test]
        public async Task ThenPriceReducedByIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.PriceReducedBy, Is.EqualTo(_apiResponse.PriceReducedBy));
        }

        [Test]
        public async Task ThenDurationReducedByHoursIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.DurationReducedByHours, Is.EqualTo(_apiResponse.DurationReducedByHours));
        }

        [Test]
        public async Task ThenIsDurationReducedByRplIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.IsDurationReducedByRpl, Is.EqualTo(_apiResponse.IsDurationReducedByRpl));
        }

        [Test]
        public async Task ThenTrainingTotalHoursIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.TrainingTotalHours, Is.EqualTo(_apiResponse.TrainingTotalHours));
        }

    }
}