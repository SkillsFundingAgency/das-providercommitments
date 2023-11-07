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
            _outerApiService.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync((long cohortId) => { _cohortResult.TransferSenderId = cohortId + 1; return _cohortResult; });

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
            Assert.IsTrue(_apiRequest.RplDataExtended);
            Assert.AreEqual(_viewModel.ProviderId, _apiRequest.ProviderId);
        }


        [Test]
        public void CommandIsReturnedFromCacheWithLogIdAsExpected()
        {
            Assert.AreEqual(_fileUploadCacheModel.FileUploadLogId, _apiRequest.FileUploadLogId);
        }

        [Test]
        public void VerifyUlnIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.Where(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.Count());
            }
        }

        [Test]
        public void VerifyFirstNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.GivenNames, result.FirstName);
            }
        }

        [Test]
        public void VerifyLastNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.FamilyName, result.LastName);
            }
        }

        [Test]
        public void VerifyDOBIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.DateOfBirth, result.DateOfBirthAsString);
            }
        }

        [Test]
        public void VerifyStartDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);                
                Assert.AreEqual(record.StartDate, result.StartDateAsString);
            }
        }

        [Test]
        public void VerifyEndDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.EndDate, result.EndDateAsString);
            }
        }

        [Test]
        public void VerifyCostIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TotalPrice, result.CostAsString);
            }
        }

        [Test]
        public void VerifyProviderRefIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.ProviderRef, result.ProviderRef);
            }
        }

        [Test]
        public void VerifyCourseCodeIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.StdCode, result.CourseCode);
            }
        }

        [Test]
        public void VerifyLegalEntityIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.LegalEntityId);
            }
        }

        [Test]
        public void VerifyCohortIdIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(2, result.CohortId);
            }
        }

        [Test]
        public void VerifyTransferSenderIdISMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual((result.CohortId +1), result.TransferSenderId);
            }
        }

        [Test]
        public void VerifyRecognisePriorLearningIsMapped()
        {
            foreach (var record in _csvRecords)
        {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.RecognisePriorLearning, result.RecognisePriorLearningAsString);
            }
        }

        [Test]
        public void VerifyDurationReducedBy()
        {
            foreach (var record in _csvRecords)
{
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.DurationReducedBy, result.DurationReducedByAsString);
            }
        }

        [Test]
        public void VerifyPriceReducedBy()
        {
            foreach (var record in _csvRecords)
{
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.PriceReducedBy, result.PriceReducedByAsString);
            }
        }

        [Test]
        public void VerifyTrainingTotalHours()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TrainingTotalHours, result.TrainingTotalHoursAsString);
            }
        }

        [Test]
        public void VerifyTrainingHoursReduction()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TrainingHoursReduction, result.TrainingHoursReductionAsString);
            }
        }

        [Test]
        public void VerifyIsDurationReducedByRPL()
        {
            foreach (var record in _csvRecords)
            {
                var result = _apiRequest.BulkUploadDraftApprenticeships.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.IsDurationReducedByRPL, result.IsDurationReducedByRPLAsString);
            }
        }

        [Test]
        public void VerifyFileUploadIdIsMappedFromCache()
        {
            Assert.AreEqual(_fileUploadCacheModel.FileUploadLogId, _apiRequest.FileUploadLogId);
        }
    }
}