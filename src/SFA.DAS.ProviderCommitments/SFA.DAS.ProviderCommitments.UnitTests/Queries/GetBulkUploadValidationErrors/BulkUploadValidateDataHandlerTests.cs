//using AutoFixture;
//using Moq;
//using NUnit.Framework;
//using SFA.DAS.CommitmentsV2.Api.Client;
//using SFA.DAS.CommitmentsV2.Api.Types.Responses;
//using SFA.DAS.CommitmentsV2.Shared.Interfaces;
//using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
//using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetBulkUploadValidationErrors
//{
//    public class BulkUploadValidateDataHandlerTests
//    {
//        [Test]
//        public void Verify_FileParser_Called()
//        {
//            // arrange
//        }

//        [Test]
//        public void Verify_Api_Mapper_Called()
//        {
//            // arrange
//        }

//        [Test]
//        public void Verify_Api_Mapper_Called()
//        {
//            // arrange
//        }

//        public class BulkUploadValidateDataHandlerTestsFixture
//        {
//            private BulkUploadValidateDataHandler BulkUploadValidateDataHandler { get; set; }
//            private Mock<ICommitmentsApiClient> _commitmentApiClient { get; set; }
//            private Mock<IModelMapper> _modelMapper { get; set; }
//            private Mock<IBulkUploadFileParser> _bulkUploadParser { get; set; }
//            private BulkUploadValidateDataRequest _bulkUploadValidateDataRequest { get; set; }
//            private BulkUploadValidateApiResponse _bulkUploadValidateApiResponse { get; set; }
//            public BulkUploadValidateDataHandlerTestsFixture()
//            {
//                var fixture = new Fixture();
//                _bulkUploadValidateDataRequest = fixture.Create<BulkUploadValidateDataRequest>();
//                _bulkUploadValidateApiResponse = fixture.Create<BulkUploadValidateApiResponse>();
//                _commitmentApiClient = new Mock<ICommitmentsApiClient>();
//                //_commitmentApiClient.Setup()
//                _modelMapper = new Mock<IModelMapper>();
//                _bulkUploadParser = new Mock<IBulkUploadFileParser>();
//                BulkUploadValidateDataHandler =  new BulkUploadValidateDataHandler(_commitmentApiClient.Object, _modelMapper.Object, _bulkUploadParser.Object);
//            }

//            public async Task<BulkUploadValidateApiResponse> Handle()
//            {
//                return await BulkUploadValidateDataHandler.Handle(_bulkUploadValidateDataRequest, CancellationToken.None);
//            }
//        }
//    }
//}
