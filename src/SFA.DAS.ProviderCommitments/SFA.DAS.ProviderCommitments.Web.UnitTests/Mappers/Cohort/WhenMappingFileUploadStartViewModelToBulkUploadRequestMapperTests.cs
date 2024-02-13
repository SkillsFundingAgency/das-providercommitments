using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingFileUploadStartViewModelToBulkUploadRequestMapperTests
    {
        private FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper _mapper;
        private Mock<ICacheService> _cacheService;
        private Mock<IEncodingService> _encodingService;
        private BulkUploadAddDraftApprenticeshipsRequest _apiRequest;
        private FileUploadReviewViewModel _viewModel;
        private List<CsvRecord> _csvRecords;
        private FileUploadCacheModel _fileUploadCacheModel;
        private Mock<IOuterApiService> _outerApiService;
        private Mock<IAuthorizationService> _authorizationService;
        private GetCohortResult _cohortResult;

        [SetUp]
        public async Task Setup()
        {
            var fixture = new Fixture();
            _apiRequest = fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            _csvRecords = fixture.Build<CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .CreateMany(2).ToList();

            _fileUploadCacheModel = new FileUploadCacheModel
            {
                CsvRecords = _csvRecords,
                FileUploadLogId = 1235
            };
            _viewModel = fixture.Build<FileUploadReviewViewModel>().Create();

            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.GetFromCache<FileUploadCacheModel>(_viewModel.CacheRequestId.ToString())).ReturnsAsync(() => _fileUploadCacheModel);

            _cohortResult = fixture.Create<GetCohortResult>();
            _outerApiService = new Mock<IOuterApiService>();
            _outerApiService.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync((long cohortId) =>
            {
                _cohortResult.TransferSenderId = cohortId + 1;
                return _cohortResult;
            });

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId)).Returns(1);
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(2);

            _authorizationService = new Mock<IAuthorizationService>();
            _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.RplExtended)).ReturnsAsync(true);

            _mapper = new FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper(_cacheService.Object, _encodingService.Object, _outerApiService.Object, _authorizationService.Object);

            _apiRequest = await _mapper.Map(_viewModel);
        }

        [Test]
        public void CommandIsReturnedWithProviderIdAndRplDataExtended()
        {
            Assert.That(_apiRequest.RplDataExtended, Is.True);
            Assert.That(_apiRequest.ProviderId, Is.EqualTo(_viewModel.ProviderId));
        }


        [Test]
        public void CommandIsReturnedFromCacheWithLogIdAsExpected()
        {
            Assert.That(_apiRequest.FileUploadLogId, Is.EqualTo(_fileUploadCacheModel.FileUploadLogId));
        }

        [Test]
        public void VerifyUlnIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.Where(x => x.Uln == record.ULN);
                Assert.That(result.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void VerifyFirstNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.FirstName, Is.EqualTo(record.GivenNames));
            }
        }

        [Test]
        public void VerifyLastNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.LastName, Is.EqualTo(record.FamilyName));
            }
        }

        [Test]
        public void VerifyDOBIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.DateOfBirthAsString, Is.EqualTo(record.DateOfBirth));
            }
        }

        [Test]
        public void VerifyStartDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.StartDateAsString, Is.EqualTo(record.StartDate));
            }
        }

        [Test]
        public void VerifyEndDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.EndDateAsString, Is.EqualTo(record.EndDate));
            }
        }

        [Test]
        public void VerifyCostIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.CostAsString, Is.EqualTo(record.TotalPrice));
            }
        }

        [Test]
        public void VerifyProviderRefIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.ProviderRef, Is.EqualTo(record.ProviderRef));
            }
        }

        [Test]
        public void VerifyCourseCodeIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.CourseCode, Is.EqualTo(record.StdCode));
            }
        }

        [Test]
        public void VerifyLegalEntityIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.LegalEntityId, Is.EqualTo(1));
            }
        }

        [Test]
        public void VerifyCohortIdIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.CohortId, Is.EqualTo(2));
            }
        }

        [Test]
        public void VerifyTransferSenderIdISMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.TransferSenderId, Is.EqualTo((result.CohortId + 1)));
            }
        }

        [Test]
        public void VerifyRecognisePriorLearningIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.RecognisePriorLearningAsString, Is.EqualTo(record.RecognisePriorLearning));
            }
        }

        [Test]
        public void VerifyDurationReducedBy()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.DurationReducedByAsString, Is.EqualTo(record.DurationReducedBy));
            }
        }

        [Test]
        public void VerifyPriceReducedBy()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.PriceReducedByAsString, Is.EqualTo(record.PriceReducedBy));
            }
        }

        [Test]
        public void VerifyTrainingTotalHours()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.TrainingTotalHoursAsString, Is.EqualTo(record.TrainingTotalHours));
            }
        }

        [Test]
        public void VerifyTrainingHoursReduction()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.TrainingHoursReductionAsString, Is.EqualTo(record.TrainingHoursReduction));
            }
        }

        [Test]
        public void VerifyIsDurationReducedByRPL()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.That(result.IsDurationReducedByRPLAsString, Is.EqualTo(record.IsDurationReducedByRPL));
            }
        }

        [Test]
        public void VerifyFileUploadIdIsMappedFromCache()
        {
            Assert.That(_apiRequest.FileUploadLogId, Is.EqualTo(_fileUploadCacheModel.FileUploadLogId));
        }
    }
}