using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingFileUploadStartViewModelToBulkUploadRequestMapperTests
    {
        private FileUploadStartViewModelToBulkUploadRequestMapper _mapper;
        private Mock<IBulkUploadFileParser> _fileParser;
        private BulkUploadAddDraftApprenticeshipsRequest _apiRequest;
        private FileUploadStartViewModel _viewModel;


        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            _apiRequest =  fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            _viewModel = fixture.Build<FileUploadStartViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            _fileParser = new Mock<IBulkUploadFileParser>();
            _fileParser.Setup(x => x.CreateApiRequest(It.IsAny<long>(), It.IsAny<IFormFile>())).Returns(() => _apiRequest);

            _mapper = new FileUploadStartViewModelToBulkUploadRequestMapper(_fileParser.Object);
        }

        [Test]
        public async Task ReturnsApiRequest()
        {
            var result = await _mapper.Map(_viewModel);
            Assert.AreEqual(_apiRequest, result);
        }

        [Test]
        public async Task FileParserIsCalledOnce()
        {
            var result = await _mapper.Map(_viewModel);
            _fileParser.Verify(x => x.CreateApiRequest(It.IsAny<long>(), It.IsAny<IFormFile>()), Times.Once);
        }

    }
}
