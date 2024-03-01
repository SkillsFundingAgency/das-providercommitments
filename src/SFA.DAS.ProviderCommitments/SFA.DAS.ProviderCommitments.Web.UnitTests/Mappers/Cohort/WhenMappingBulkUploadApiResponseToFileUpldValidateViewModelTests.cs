using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using BulkUploadValidationError = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError;

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
        private List<BulkUploadValidationError> _errors;

        [SetUp]
        public async Task Setup()
        {
            _guidCacheError = Guid.NewGuid().ToString();
            _source = new FileUploadValidateErrorRequest
            {
                CachedErrorGuid = _guidCacheError
            };

            var errorsFirstRow = new List<Error>
            {
                new("Property1", "First Error Text"),
                new("Property2", "Second Error Text")
            };

            var errorsSecondRow = new List<Error>
            {
                new("Property12", "First Error Text2"),
                new("Property22", "Second Error Text2")
            };

            _errors = new List<BulkUploadValidationError>
                 {
                      new(1, "EmployerName","ULN", "apprentice name", errorsFirstRow),
                      new(2, "EmployerName2","ULN2", "apprentice name2", errorsSecondRow),
                };

            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.GetFromCache<List<BulkUploadValidationError>>(_guidCacheError)).ReturnsAsync(_errors);

            _mapper = new BulkUploadValidateApiResponseToFileUpldValidateViewModel(_cacheService.Object);
            _result = await _mapper.Map(_source);
        }

        [Test]
        public void EmployerName_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.BulkUploadValidationErrors.First().RowNumber, Is.EqualTo(1));
                Assert.That(_result.BulkUploadValidationErrors.Last().RowNumber, Is.EqualTo(2));
            });
        }

        [Test]
        public void ProviderId_Is_Mapped()
        {
            Assert.That(_result.ProviderId, Is.EqualTo(_source.ProviderId));
        }

        [Test]
        public void ApprenticeName_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.BulkUploadValidationErrors.First().ApprenticeName, Is.EqualTo(_errors.First().ApprenticeName));
                Assert.That(_result.BulkUploadValidationErrors.Last().ApprenticeName, Is.EqualTo(_errors.Last().ApprenticeName));
            });
        }

        [Test]
        public void Uln_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.BulkUploadValidationErrors.First().Uln, Is.EqualTo(_errors.First().Uln));
                Assert.That(_result.BulkUploadValidationErrors.Last().Uln, Is.EqualTo(_errors.Last().Uln));
            });
        }

        [Test]
        public void Error_Text_Are_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.BulkUploadValidationErrors.First().PropertyErrors.First().ErrorText, Is.EqualTo(_errors.First().Errors.First().ErrorText));
                Assert.That(_result.BulkUploadValidationErrors.First().PropertyErrors.Last().ErrorText, Is.EqualTo(_errors.First().Errors.Last().ErrorText));

                Assert.That(_result.BulkUploadValidationErrors.Last().PropertyErrors.First().ErrorText, Is.EqualTo(_errors.Last().Errors.First().ErrorText));
                Assert.That(_result.BulkUploadValidationErrors.Last().PropertyErrors.Last().ErrorText, Is.EqualTo(_errors.Last().Errors.Last().ErrorText));
            });
        }

        [Test]
        public void Error_Property_Are_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.BulkUploadValidationErrors.First().PropertyErrors.First().Property, Is.EqualTo(_errors.First().Errors.First().Property));
                Assert.That(_result.BulkUploadValidationErrors.First().PropertyErrors.Last().Property, Is.EqualTo(_errors.First().Errors.Last().Property));

                Assert.That(_result.BulkUploadValidationErrors.Last().PropertyErrors.First().Property, Is.EqualTo(_errors.Last().Errors.First().Property));
                Assert.That(_result.BulkUploadValidationErrors.Last().PropertyErrors.Last().Property, Is.EqualTo(_errors.Last().Errors.Last().Property));
            });
        }
    }
}
