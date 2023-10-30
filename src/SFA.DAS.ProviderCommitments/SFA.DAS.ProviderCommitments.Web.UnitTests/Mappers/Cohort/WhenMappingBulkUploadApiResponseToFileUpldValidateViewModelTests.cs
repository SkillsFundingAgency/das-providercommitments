using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingBulkUploadApiResponseToFileUpldValidateViewModelTests
    {
        private BulkUploadValidateApiResponseToFileUpldValidateViewModel _mapper;
        private FileUploadValidateViewModel _result;
        private FileUploadValidateErrorRequest _source;
        private Mock<ICacheService> _cacheService;
        private string _guidCacheError;
        private List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError> _errors;

        [SetUp]
        public async Task Setup()
        {
            _guidCacheError = Guid.NewGuid().ToString();
            _source = new FileUploadValidateErrorRequest();
            _source.CachedErrorGuid = _guidCacheError;
            
            var errorsFirstRow = new List<Infrastructure.OuterApi.ErrorHandling.Error>
            {
                new("Property1", "First Error Text"),
                new("Property2", "Second Error Text")
            };

            var errorsSecondRow = new List<Infrastructure.OuterApi.ErrorHandling.Error>
            {
                new("Property12", "First Error Text2"),
                new("Property22", "Second Error Text2")
            };

            _errors = new List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError>
                 {
                      new(1, "EmployerName","ULN", "apprentice name", errorsFirstRow),
                      new(2, "EmployerName2","ULN2", "apprentice name2", errorsSecondRow),
                };

            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.GetFromCache<List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError>>(_guidCacheError)).ReturnsAsync(_errors);

            _mapper = new BulkUploadValidateApiResponseToFileUpldValidateViewModel(_cacheService.Object);
            _result = await _mapper.Map(_source);
        }

        [Test]
        public void EmployerName_Is_Mapped()
        {
            Assert.AreEqual(1, _result.BulkUploadValidationErrors.First().RowNumber);
            Assert.AreEqual(2, _result.BulkUploadValidationErrors.Last().RowNumber);
        }

        [Test]
        public void ProviderId_Is_Mapped()
        {
            Assert.AreEqual(_source.ProviderId, _result.ProviderId);
        }

        [Test]
        public void ApprenticeName_Is_Mapped()
        {
            Assert.AreEqual(_errors.First().ApprenticeName, _result.BulkUploadValidationErrors.First().ApprenticeName);
            Assert.AreEqual(_errors.Last().ApprenticeName, _result.BulkUploadValidationErrors.Last().ApprenticeName);
        }

        [Test]
        public void Uln_Is_Mapped()
        {
            Assert.AreEqual(_errors.First().Uln, _result.BulkUploadValidationErrors.First().Uln);
            Assert.AreEqual(_errors.Last().Uln, _result.BulkUploadValidationErrors.Last().Uln);
        }

        [Test]
        public void Error_Text_Are_Mapped()
        {
            Assert.AreEqual(_errors.First().Errors.First().ErrorText, _result.BulkUploadValidationErrors.First().PropertyErrors.First().ErrorText);
            Assert.AreEqual(_errors.First().Errors.Last().ErrorText, _result.BulkUploadValidationErrors.First().PropertyErrors.Last().ErrorText);
                                    
            Assert.AreEqual(_errors.Last().Errors.First().ErrorText, _result.BulkUploadValidationErrors.Last().PropertyErrors.First().ErrorText);
            Assert.AreEqual(_errors.Last().Errors.Last().ErrorText, _result.BulkUploadValidationErrors.Last().PropertyErrors.Last().ErrorText);
        }

        [Test]
        public void Error_Property_Are_Mapped()
        {
            Assert.AreEqual(_errors.First().Errors.First().Property, _result.BulkUploadValidationErrors.First().PropertyErrors.First().Property);
            Assert.AreEqual(_errors.First().Errors.Last().Property, _result.BulkUploadValidationErrors.First().PropertyErrors.Last().Property);
                                    
            Assert.AreEqual(_errors.Last().Errors.First().Property, _result.BulkUploadValidationErrors.Last().PropertyErrors.First().Property);
            Assert.AreEqual(_errors.Last().Errors.Last().Property, _result.BulkUploadValidationErrors.Last().PropertyErrors.Last().Property);
        }
    }
}
