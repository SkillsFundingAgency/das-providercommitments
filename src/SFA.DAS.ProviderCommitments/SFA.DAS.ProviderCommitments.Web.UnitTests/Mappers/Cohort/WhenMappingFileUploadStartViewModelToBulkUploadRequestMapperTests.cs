using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
        private List<Web.Models.Cohort.CsvRecord> _csvRecords;

        [SetUp]
        public async Task Setup()
        {
            var fixture = new Fixture();
            _apiRequest = fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            _csvRecords = fixture.Build<Web.Models.Cohort.CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .CreateMany(2).ToList();
            _viewModel = fixture.Build<FileUploadReviewViewModel>().Create();
               
            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.GetFromCache<List<Web.Models.Cohort.CsvRecord>>(_viewModel.CacheRequestId.ToString())).ReturnsAsync(() => _csvRecords);

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId)).Returns(1);
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(2);

            _mapper = new FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper(  _cacheService.Object, _encodingService.Object);

            _apiRequest = await _mapper.Map(_viewModel);
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
                Assert.AreEqual(GetValidDate(record.DateOfBirth, "yyyy-MM-dd").Value.Date, result.DateOfBirth);
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

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }
}
