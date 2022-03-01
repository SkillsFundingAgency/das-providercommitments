using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetBulkUploadValidationErrors
{
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

        public class BulkUploadValidateDataHandlerTestsFixture
        {
            private FileUploadValidateDataHandler BulkUploadValidateDataHandler { get; set; }
            private Mock<ICommitmentsApiClient> _commitmentApiClient { get; set; }
            private Mock<IModelMapper> _modelMapper { get; set; }
            private Mock<IBulkUploadFileParser> _bulkUploadParser { get; set; }
            private FileUploadValidateDataRequest _bulkUploadValidateDataRequest { get; set; }
            private BulkUploadValidateApiResponse _bulkUploadValidateApiResponse { get; set; }
            private BulkUploadValidateApiRequest _bulkUploadValidateApiRequest { get; set; }
            public BulkUploadValidateDataHandlerTestsFixture()
            {
                var fixture = new Fixture();
                _bulkUploadValidateDataRequest = new FileUploadValidateDataRequest() ;
                _bulkUploadValidateApiResponse = fixture.Create<BulkUploadValidateApiResponse>();
                _bulkUploadValidateApiRequest = fixture.Create<BulkUploadValidateApiRequest>();

                _commitmentApiClient = new Mock<ICommitmentsApiClient>();
                _commitmentApiClient.Setup(x => x.ValidateBulkUploadRequest(It.IsAny<long>(), It.IsAny<BulkUploadValidateApiRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => _bulkUploadValidateApiResponse);
                
                _modelMapper = new Mock<IModelMapper>();
                _modelMapper.Setup(x => x.Map<BulkUploadValidateApiRequest>(_bulkUploadValidateDataRequest)).ReturnsAsync(() => _bulkUploadValidateApiRequest);
                
                _bulkUploadParser = new Mock<IBulkUploadFileParser>();
                _bulkUploadParser.Setup(x => x.GetCsvRecords(It.IsAny<long>(), It.IsAny<IFormFile>()))
                    .Returns(new System.Collections.Generic.List<Web.Models.Cohort.CsvRecord>());
                BulkUploadValidateDataHandler = new FileUploadValidateDataHandler(_commitmentApiClient.Object, _modelMapper.Object, _bulkUploadParser.Object);
            }

            public async Task<BulkUploadValidateApiResponse> Handle()
            {
                return await BulkUploadValidateDataHandler.Handle(_bulkUploadValidateDataRequest, CancellationToken.None);
            }

            internal void VerifyApiMapperCalled()
            {
                _modelMapper.Verify(x => x.Map<BulkUploadValidateApiRequest>(_bulkUploadValidateDataRequest), Times.Once);
            }

            internal void VerifyBulkUploadDataValidated()
            {
                _commitmentApiClient.Verify(x => x.ValidateBulkUploadRequest(It.IsAny<long>(), It.IsAny<BulkUploadValidateApiRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
