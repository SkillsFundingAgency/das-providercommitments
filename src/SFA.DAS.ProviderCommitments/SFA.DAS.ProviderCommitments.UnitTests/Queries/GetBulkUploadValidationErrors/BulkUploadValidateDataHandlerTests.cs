using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetBulkUploadValidationErrors;

[TestFixture]
public class BulkUploadValidateDataHandlerTests
{
    [Test]
    public async Task Verify_Api_Mapper_Called()
    {
        var fixture = new BulkUploadValidateDataHandlerTestsFixture();
        await fixture.Handle();

        fixture.VerifyApiMapperCalled();
    }

    [Test]
    public async Task Verify_Bulkupload_Validated()
    {
        var fixture = new BulkUploadValidateDataHandlerTestsFixture();
        await fixture.Handle();

        fixture.VerifyBulkUploadDataValidated();
    }

    private class BulkUploadValidateDataHandlerTestsFixture
    {
        private readonly FileUploadValidateDataHandler _bulkUploadValidateDataHandler;
        private readonly Mock<IOuterApiService> _outerApiService;
        private readonly Mock<IModelMapper> _modelMapper;
        private readonly Mock<IBulkUploadFileParser> _bulkUploadParser;
        private readonly FileUploadValidateDataRequest _bulkUploadValidateDataRequest;
        private readonly BulkUploadValidateApimRequest _bulkUploadValidateApiRequest;
        private readonly Mock<IAuthorizationService> _authorizationService;
        
        public BulkUploadValidateDataHandlerTestsFixture()
        {
            var fixture = new Fixture();
            _bulkUploadValidateDataRequest = new FileUploadValidateDataRequest() ;
            _bulkUploadValidateApiRequest = fixture.Create<BulkUploadValidateApimRequest>();
            _authorizationService = new Mock<IAuthorizationService>();

            _outerApiService = new Mock<IOuterApiService>();

            _modelMapper = new Mock<IModelMapper>();
            _modelMapper.Setup(x => x.Map<BulkUploadValidateApimRequest>(_bulkUploadValidateDataRequest)).ReturnsAsync(() => _bulkUploadValidateApiRequest);
                
            _bulkUploadParser = new Mock<IBulkUploadFileParser>();
            _bulkUploadParser.Setup(x => x.GetCsvRecords(It.IsAny<long>(), It.IsAny<IFormFile>()))
                .Returns(new System.Collections.Generic.List<CsvRecord>());
            _bulkUploadValidateDataHandler = new FileUploadValidateDataHandler(_outerApiService.Object, _modelMapper.Object, _bulkUploadParser.Object, _authorizationService.Object);
        }

        public async Task Handle()
        {
            await _bulkUploadValidateDataHandler.Handle(_bulkUploadValidateDataRequest, CancellationToken.None);
        }

        internal void VerifyApiMapperCalled()
        {
            _modelMapper.Verify(x => x.Map<BulkUploadValidateApimRequest>(_bulkUploadValidateDataRequest), Times.Once);
        }

        internal void VerifyBulkUploadDataValidated()
        {
            _outerApiService.Verify(x => x.ValidateBulkUploadRequest(It.IsAny<BulkUploadValidateApimRequest>()), Times.Once);
        }
    }
}