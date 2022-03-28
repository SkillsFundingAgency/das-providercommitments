using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingBulkUploadApiResponseToFileUpldValidateViewModelTests
    {
        private BulkUploadValidateApiResponseToFileUpldValidateViewModel _mapper;
        private FileUploadValidateViewModel _result;
        private List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError> _source;

        [SetUp]
        public async Task Setup()
        {
            var errorsFirstRow = new List<Error>();
            errorsFirstRow.Add(new Error("Property1", "First Error Text"));
            errorsFirstRow.Add(new Error("Property2", "Second Error Text"));

            var errorsSecondRow = new List<Error>();
            errorsSecondRow.Add(new Error("Property12", "First Error Text2"));
            errorsSecondRow.Add(new Error("Property22", "Second Error Text2"));

            _source = new List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError>
                 {
                      new CommitmentsV2.Api.Types.Responses.BulkUploadValidationError(1, "EmployerName","ULN", "apprentice name", errorsFirstRow),
                      new CommitmentsV2.Api.Types.Responses.BulkUploadValidationError(2, "EmployerName2","ULN2", "apprentice name2", errorsSecondRow),
                };

            _mapper = new BulkUploadValidateApiResponseToFileUpldValidateViewModel();
            _result = await _mapper.Map(_source);
        }

        [Test]
        public async Task EmployerName_Is_Mapped()
        {
            Assert.AreEqual(1, _result.BulkUploadValidationErrors.First().RowNumber);
            Assert.AreEqual(2, _result.BulkUploadValidationErrors.Last().RowNumber);
        }

        [Test]
        public async Task ApprenticeName_Is_Mapped()
        {
            Assert.AreEqual(_source.First().ApprenticeName, _result.BulkUploadValidationErrors.First().ApprenticeName);
            Assert.AreEqual(_source.Last().ApprenticeName, _result.BulkUploadValidationErrors.Last().ApprenticeName);
        }

        [Test]
        public async Task Uln_Is_Mapped()
        {
            Assert.AreEqual(_source.First().Uln, _result.BulkUploadValidationErrors.First().Uln);
            Assert.AreEqual(_source.Last().Uln, _result.BulkUploadValidationErrors.Last().Uln);
        }

        [Test]
        public async Task Error_Text_Are_Mapped()
        {
            Assert.AreEqual(_source.First().Errors.First().ErrorText, _result.BulkUploadValidationErrors.First().PropertyErrors.First().ErrorText);
            Assert.AreEqual(_source.First().Errors.Last().ErrorText, _result.BulkUploadValidationErrors.First().PropertyErrors.Last().ErrorText);

            Assert.AreEqual(_source.Last().Errors.First().ErrorText, _result.BulkUploadValidationErrors.Last().PropertyErrors.First().ErrorText);
            Assert.AreEqual(_source.Last().Errors.Last().ErrorText, _result.BulkUploadValidationErrors.Last().PropertyErrors.Last().ErrorText);
        }

        [Test]
        public async Task Error_Proeprty_Are_Mapped()
        {
            Assert.AreEqual(_source.First().Errors.First().Property, _result.BulkUploadValidationErrors.First().PropertyErrors.First().Property);
            Assert.AreEqual(_source.First().Errors.Last().Property, _result.BulkUploadValidationErrors.First().PropertyErrors.Last().Property);

            Assert.AreEqual(_source.Last().Errors.First().Property, _result.BulkUploadValidationErrors.Last().PropertyErrors.First().Property);
            Assert.AreEqual(_source.Last().Errors.Last().Property, _result.BulkUploadValidationErrors.Last().PropertyErrors.Last().Property);
        }
    }
}
