using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.DraftApprenticeshipTests
{
    [TestFixture]
    public class WhenIMapViewDraftApprenticeshipDetailsToViewModel
    {
        private ViewDraftApprenticeshipViewModelMapper _mapper;
        private DraftApprenticeshipRequest _source;
        public Mock<IOuterApiClient> OuterApiClient;
        private Func<Task<ViewDraftApprenticeshipViewModel>> _act;
        private Infrastructure.OuterApi.Requests.Cohorts.GetViewDraftApprenticeshipResponse _apiResponse;
        private GetTrainingProgrammeResponse _apiTrainingProgrammeResponse;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _apiResponse = fixture.Build<Infrastructure.OuterApi.Requests.Cohorts.GetViewDraftApprenticeshipResponse>().Create();
            _apiTrainingProgrammeResponse = fixture.Build<GetTrainingProgrammeResponse>().Create();

            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

            commitmentsApiClient.Setup(x => x.GetTrainingProgramme(_apiResponse.CourseCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apiTrainingProgrammeResponse);

            OuterApiClient = new Mock<IOuterApiClient>();
            OuterApiClient.Setup(x => x.Get<Infrastructure.OuterApi.Requests.Cohorts.GetViewDraftApprenticeshipResponse>(It.IsAny<GetViewDraftApprenticeshipRequest>()))
                .ReturnsAsync(_apiResponse);

            _mapper = new ViewDraftApprenticeshipViewModelMapper(commitmentsApiClient.Object, OuterApiClient.Object);
            _source = fixture.Build<DraftApprenticeshipRequest>().Create();

            _act = async () => (await _mapper.Map(TestHelper.Clone(_source))) as ViewDraftApprenticeshipViewModel;
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenCohortReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task ThenFirstNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.FirstName, result.FirstName);
        }

        [Test]
        public async Task ThenLastNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.LastName, result.LastName);
        }

        [Test]
        public async Task ThenEmailIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.Email, result.Email);
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
            Assert.AreEqual(_apiResponse.DateOfBirth, result.DateOfBirth);
        }

        [TestCase(Infrastructure.OuterApi.Types.DeliveryModel.Regular)]
        [TestCase(Infrastructure.OuterApi.Types.DeliveryModel.PortableFlexiJob)]
        public async Task ThenDeliveryModelIsMappedCorrectly(Infrastructure.OuterApi.Types.DeliveryModel dm)
        {
            _apiResponse.DeliveryModel = dm;
            var result = await _act();
            Assert.AreEqual(dm, result.DeliveryModel);
        }

        [Test]
        public async Task ThenCourseNameIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiTrainingProgrammeResponse.TrainingProgramme.Name, result.TrainingCourse);
        }

        [Test]
        public async Task ThenEmploymentPriceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EmploymentPrice, result.EmploymentPrice);
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
            Assert.AreEqual(_apiResponse.StartDate, result.StartDate);
        }

        [Test]
        public async Task ThenEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EndDate, result.EndDate);
        }

        [Test]
        public async Task ThenEmploymentEndDateIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.EmploymentEndDate, result.EmploymentEndDate);
        }

        [Test]
        public async Task ThenOriginatorReferenceIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.Reference, result.Reference);
        }
        
        [Test]
        public async Task ThenTheVersionIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.TrainingCourseVersion, result.TrainingCourseVersion);
        }
                
        [Test]
        public async Task ThenTheSelectedOptionIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.TrainingCourseOption, result.TrainingCourseOption);
        }
        
        [Test]
        public async Task ThenTheSelectedOptionIsDisplayedAsToBeConfirmedIfEmpty()
        {
            _apiResponse.TrainingCourseOption = string.Empty;
            var result = await _act();
            Assert.AreEqual("To be confirmed", result.TrainingCourseOption);
        }
        
        [Test]
        public async Task ThenTheSelectedOptionIsDisplayedAsEmptyIfNullFromApi()
        {
            _apiResponse.TrainingCourseOption = null;
            var result = await _act();
            Assert.AreEqual("", result.TrainingCourseOption);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenHasOptionsIsMappedCorrectly(bool hasOptions)
        {
            _apiResponse.HasStandardOptions = hasOptions;
            var result = await _act();
            Assert.AreEqual(hasOptions, result.HasTrainingCourseOption);
        }
    }
}