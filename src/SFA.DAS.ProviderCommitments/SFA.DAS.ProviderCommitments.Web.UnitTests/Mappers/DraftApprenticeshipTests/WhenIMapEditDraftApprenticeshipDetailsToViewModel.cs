using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Features;
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
        private GetDraftApprenticeshipResponse _apiResponse;
        private Mock<IAuthorizationService> _authorizationService;
        private string _cohortReference;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _cohortReference = fixture.Create<string>();

            _apiResponse = fixture.Build<GetDraftApprenticeshipResponse>().Create();
            var commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            commitmentsApiClient.Setup(x =>
                    x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_apiResponse);

            _authorizationService = new Mock<IAuthorizationService>();

            _mapper = new EditDraftApprenticeshipViewModelMapper(commitmentsApiClient.Object, _authorizationService.Object);
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
            Assert.AreEqual(_apiResponse.Reference, result.Reference);
        }

        [Test]
        public async Task ThenProviderIdIsMappedCorrectly()
        {
            var result = await _act();
            Assert.AreEqual(_source.Request.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task ThenTheStandardUIdIsMapped()
        {
            var result = await _act();
            Assert.AreEqual(_apiResponse.StandardUId, result.StandardUId);
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

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenShowEmailIsMappedCorrectly(bool showEmail)
        {
            _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.ApprenticeEmail)).ReturnsAsync(showEmail);
            var result = await _act();
            Assert.AreEqual(showEmail, result.ShowEmail);
        }
    }
}