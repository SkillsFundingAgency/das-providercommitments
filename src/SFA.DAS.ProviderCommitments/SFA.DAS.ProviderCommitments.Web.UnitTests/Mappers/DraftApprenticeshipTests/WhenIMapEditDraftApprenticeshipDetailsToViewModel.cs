using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
    [TestFixture]
    public class WhenIMapEditDraftApprenticeshipDetailsToViewModel
    {
        private EditDraftApprenticeshipViewModelMapper _mapper;
        private EditDraftApprenticeshipRequest _source;
        private Func<Task<EditDraftApprenticeshipViewModel>> _act;
        private GetEditDraftApprenticeshipResponse _apiResponse;
        private string _cohortReference;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _cohortReference = fixture.Create<string>();

            _apiResponse = fixture.Build<GetEditDraftApprenticeshipResponse>().Create();
            var commitmentsApiClient = new Mock<IOuterApiClient>();
            commitmentsApiClient.Setup(x =>
                    x.Get<GetEditDraftApprenticeshipResponse>(It.IsAny<GetEditDraftApprenticeshipRequest>()))
                .ReturnsAsync(_apiResponse);

            _mapper = new EditDraftApprenticeshipViewModelMapper(Mock.Of<IEncodingService>(), commitmentsApiClient.Object);
            _source = fixture.Build<EditDraftApprenticeshipRequest>().Create();

            _act = async () => (await _mapper.Map(TestHelper.Clone(_source))) as EditDraftApprenticeshipViewModel;
        }

        [Test]
        public async Task ThenDraftApprenticeshipIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Request.DraftApprenticeshipId, result.DraftApprenticeshipId);
        }

        [Test]
        public async Task ThenCohortIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Request.CohortId, result.CohortId);
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Request.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task ThenReservationIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.ReservationId, result.ReservationId);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.Email, result.Email);
        }

        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.LastName, result.LastName);
        }

        [Test]
        public async Task ThenUniqueLearnerNumberIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.Uln, result.Uln);
        }

        [Test]
        public async Task ThenDateOfBirthIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.DateOfBirth?.Day, result.DateOfBirth.Day);
            Assert.AreEqual(_apiResponse.DateOfBirth?.Month, result.DateOfBirth.Month);
            Assert.AreEqual(_apiResponse.DateOfBirth?.Year, result.DateOfBirth.Year);
        }

        [Test]
        public async Task ThenCourseCodeIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task ThenCostIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.Cost, result.Cost);
        }

        [Test]
        public async Task ThenStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.StartDate?.Month, result.StartDate.Month);
            Assert.AreEqual(_apiResponse.StartDate?.Year, result.StartDate.Year);
        }

        [Test]
        public async Task ThenActualStartDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.ActualStartDate?.Month, result.ActualStartDate.Month);
            Assert.AreEqual(_apiResponse.ActualStartDate?.Day, result.ActualStartDate.Day);
            Assert.AreEqual(_apiResponse.ActualStartDate?.Year, result.ActualStartDate.Year);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EndDate?.Month, result.EndDate.Month);
            Assert.AreEqual(_apiResponse.EndDate?.Year, result.EndDate.Year);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.ProviderReference, result.Reference);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Request.ProviderId, result.ProviderId);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenTheTrainingCourseOptionIsMapped(bool hasStandardOptions)
        {
            _apiResponse.HasStandardOptions = hasStandardOptions;

            var result = await _act();
            
            Assert.AreEqual(_apiResponse.HasStandardOptions, result.HasStandardOptions);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenIsContinuationIsMappedCorrectly(bool isContinuation)
        {
            _apiResponse.IsContinuation = isContinuation;
            var result = await _act();
            Assert.AreEqual(_apiResponse.IsContinuation, result.IsContinuation);
        }

        [Test]
        public async Task ThenTheTrainingCourseOptionIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.TrainingCourseOption, result.TrainingCourseOption);
        }
        
        [Test]
        public async Task ThenTheTrainingCourseOptionIsMappedToMinusOneIfEmpty()
        {
            _apiResponse.TrainingCourseOption = string.Empty;
            var result = await _act();
            Assert.AreEqual("-1", result.TrainingCourseOption);
        }

        [TestCase(DeliveryModel.Regular)]
        [TestCase(DeliveryModel.PortableFlexiJob)]
        public async Task ThenDeliveryModelIsMappedCorrectly(DeliveryModel dm)
        {
            _apiResponse.DeliveryModel = (Infrastructure.OuterApi.Types.DeliveryModel) dm;
            var result = await _act();
            Assert.AreEqual(dm, result.DeliveryModel);
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            _apiResponse.EmploymentPrice = 1234;
            var result = await _act();
            Assert.AreEqual(1234, result.EmploymentPrice);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EmploymentEndDate?.Month, result.EmploymentEndDate.Month);
            Assert.AreEqual(_apiResponse.EmploymentEndDate?.Year, result.EmploymentEndDate.Year);
        }

        [Test]
        public async Task ThenRecognisePriorLearningIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.RecognisePriorLearning, result.RecognisePriorLearning);
        }

        [Test]
        public async Task ThenDurationReducedByIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.DurationReducedBy, result.DurationReducedBy);
        }

        [Test]
        public async Task ThenPriceReducedByIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.PriceReducedBy, result.PriceReducedBy);
        }

        [Test]
        public async Task ThenRecognisingPriorLearningStillNeedsToBeConsideredIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered, result.RecognisingPriorLearningStillNeedsToBeConsidered);
        }
    }
}