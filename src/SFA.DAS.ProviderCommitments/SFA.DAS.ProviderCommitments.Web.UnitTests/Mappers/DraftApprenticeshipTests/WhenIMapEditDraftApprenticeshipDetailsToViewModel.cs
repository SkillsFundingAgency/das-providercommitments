using System;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
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
            Assert.That(result.DraftApprenticeshipId, Is.EqualTo(_source.Request.DraftApprenticeshipId));
        }

        [Test]
        public async Task ThenCohortIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CohortId, Is.EqualTo(_source.Request.CohortId));
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CohortReference, Is.EqualTo(_source.Request.CohortReference));
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ReservationId, Is.EqualTo(_apiResponse.ReservationId));
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.FirstName, Is.EqualTo(_apiResponse.FirstName));
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Email, Is.EqualTo(_apiResponse.Email));
        }

        [Test]
        public async Task ThenEmailAddressConfirmedIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmailAddressConfirmed, Is.EqualTo(_apiResponse.EmailAddressConfirmed));
        }


        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.LastName, Is.EqualTo(_apiResponse.LastName));
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
            Assert.Multiple(() =>
            {
                Assert.That(result.DateOfBirth.Day, Is.EqualTo(_apiResponse.DateOfBirth?.Day));
                Assert.That(result.DateOfBirth.Month, Is.EqualTo(_apiResponse.DateOfBirth?.Month));
                Assert.That(result.DateOfBirth.Year, Is.EqualTo(_apiResponse.DateOfBirth?.Year));
            });
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.CourseCode, Is.EqualTo(_apiResponse.CourseCode));
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Cost, Is.EqualTo(_apiResponse.Cost));
        }

        [Test]
        public async Task ThenTrainingPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.TrainingPrice, Is.EqualTo(_apiResponse.TrainingPrice));
        }

        [Test]
        public async Task ThenEndPointAssessmentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(_apiResponse.EndPointAssessmentPrice));
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.Multiple(() =>
            {
                Assert.That(result.StartDate.Month, Is.EqualTo(_apiResponse.StartDate?.Month));
                Assert.That(result.StartDate.Year, Is.EqualTo(_apiResponse.StartDate?.Year));
            });
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.Multiple(() =>
            {
                Assert.That(result.ActualStartDate.Month, Is.EqualTo(_apiResponse.ActualStartDate?.Month));
                Assert.That(result.ActualStartDate.Day, Is.EqualTo(_apiResponse.ActualStartDate?.Day));
                Assert.That(result.ActualStartDate.Year, Is.EqualTo(_apiResponse.ActualStartDate?.Year));
            });
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.Multiple(() =>
            {
                Assert.That(result.EndDate.Month, Is.EqualTo(_apiResponse.EndDate?.Month));
                Assert.That(result.EndDate.Year, Is.EqualTo(_apiResponse.EndDate?.Year));
            });
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.Reference, Is.EqualTo(_apiResponse.ProviderReference));
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.ProviderId, Is.EqualTo(_source.Request.ProviderId));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenTheTrainingCourseOptionIsMapped(bool hasStandardOptions)
        {
            _apiResponse.HasStandardOptions = hasStandardOptions;

            var result = await _act();

            Assert.That(result.HasStandardOptions, Is.EqualTo(_apiResponse.HasStandardOptions));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenIsContinuationIsMappedCorrectly(bool isContinuation)
        {
            _apiResponse.IsContinuation = isContinuation;
            var result = await _act();
            Assert.That(result.IsContinuation, Is.EqualTo(_apiResponse.IsContinuation));
        }

        [Test]
        public async Task ThenTheTrainingCourseOptionIsMapped()
        {
            var result = await _act();
            Assert.That(result.TrainingCourseOption, Is.EqualTo(_apiResponse.TrainingCourseOption));
        }
        
        [Test]
        public async Task ThenTheTrainingCourseOptionIsMappedToMinusOneIfEmpty()
        {
            _apiResponse.TrainingCourseOption = string.Empty;
            var result = await _act();
            Assert.That(result.TrainingCourseOption, Is.EqualTo("-1"));
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task ThenDeliveryModelIsMappedCorrectly(DeliveryModel dm)
        {
            _apiResponse.DeliveryModel = (Infrastructure.OuterApi.Types.DeliveryModel) dm;
            var result = await _act();
            Assert.That(result.DeliveryModel, Is.EqualTo(dm));
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            _apiResponse.EmploymentPrice = 1234;
            var result = await _act();
            Assert.That(result.EmploymentPrice, Is.EqualTo(1234));
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.Multiple(() =>
            {
                Assert.That(result.EmploymentEndDate.Month, Is.EqualTo(_apiResponse.EmploymentEndDate?.Month));
                Assert.That(result.EmploymentEndDate.Year, Is.EqualTo(_apiResponse.EmploymentEndDate?.Year));
            });
        }

        [Test]
        public async Task ThenRecognisePriorLearningIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.RecognisePriorLearning, Is.EqualTo(_apiResponse.RecognisePriorLearning));
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
        public async Task ThenRecognisingPriorLearningStillNeedsToBeConsideredIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.RecognisingPriorLearningStillNeedsToBeConsidered, Is.EqualTo(_apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered));
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
            Assert.That(result.HasUnavailableFlexiJobAgencyDeliveryModel, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task ThenIsOnFlexiPaymentPilotIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(_apiResponse.IsOnFlexiPaymentPilot));
        }

        [Test]
        public async Task ThenEmployerHasEditedCostIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.EmployerHasEditedCost, Is.EqualTo(_apiResponse.EmployerHasEditedCost));
        }

        [Test]
        public async Task ThenHasMultipleDeliveryModelOptionsIsMappedCorrectly()
        {
            var result = await _act();
            Assert.That(result.HasMultipleDeliveryModelOptions, Is.EqualTo(_apiResponse.HasMultipleDeliveryModelOptions));
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
            Assert.That(result.HasChangedDeliveryModel, Is.EqualTo(expectedResult));
        }
    }
}