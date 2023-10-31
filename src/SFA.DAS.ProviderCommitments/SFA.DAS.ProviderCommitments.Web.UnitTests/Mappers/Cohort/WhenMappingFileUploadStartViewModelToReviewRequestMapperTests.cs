using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingFileUploadStartViewModelToReviewRequestMapperTests
    {
        private FileUploadStartViewModelToReviewRequestMapper _mapper;
        private Mock<IBulkUploadFileParser> _fileParser;
        private List<CsvRecord> _csvRecords;
        private FileUploadStartViewModel _viewModel;
        private Mock<ICacheService> _cacheService;
        private Guid _cacheRequestId;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            _csvRecords = fixture.Create<List<CsvRecord>>();
            _viewModel = fixture.Build<FileUploadStartViewModel>()
                .With(x => x.Attachment, Mock.Of<IFormFile>()).Create();
            _fileParser = new Mock<IBulkUploadFileParser>();
            _fileParser.Setup(x => x.GetCsvRecords(It.IsAny<long>(), It.IsAny<IFormFile>())).Returns(() => _csvRecords);
            _cacheRequestId = Guid.NewGuid();
            _cacheService = new Mock<ICacheService>();
            _cacheService.Setup(x => x.SetCache(_csvRecords, It.IsAny<string>())).ReturnsAsync(_cacheRequestId);

            _mapper = new FileUploadStartViewModelToReviewRequestMapper(_fileParser.Object, _cacheService.Object);
        }

        [Test]
        public async Task FileParserIsCalledOnce()
        {
            var result = await _mapper.Map(_viewModel);
            _fileParser.Verify(x => x.GetCsvRecords(It.IsAny<long>(), It.IsAny<IFormFile>()), Times.Once);
        }

        [Test]
        public async Task CacheServiceIsCalledOnce()
        {
            var result = await _mapper.Map(_viewModel);
            _cacheService.Verify(x => x.SetCache(_csvRecords, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CachedRequestIdIsMapped()
        {
            var result = await _mapper.Map(_viewModel);
            Assert.AreEqual(_cacheRequestId, result.CacheRequestId);
        }

        [Test]
        public async Task ProviderIdIsMapped()
        {
            var result = await _mapper.Map(_viewModel);
            Assert.AreEqual(_viewModel.ProviderId, result.ProviderId);
        }
    }
}
