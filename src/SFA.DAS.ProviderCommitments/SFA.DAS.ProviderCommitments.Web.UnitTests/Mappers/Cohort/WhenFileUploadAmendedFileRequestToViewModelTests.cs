using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenFileUploadAmendedFileRequestToViewModelTests
    {
        private FileUploadAmendedFileRequestToViewModel _mapper;
        private FileUploadAmendedFileRequest _request;
        private FileUploadAmendedFileViewModel _viewModel;

        [SetUp]
        public async Task Setup()
        {
            var fixture = new Fixture();
            _request = fixture.Create<FileUploadAmendedFileRequest>();
            _mapper = new FileUploadAmendedFileRequestToViewModel();
            _viewModel = await _mapper.Map(_request);
        }

        [Test]
        public void VerifyProviderIdIsMapped()
        {
            Assert.That(_viewModel.ProviderId, Is.EqualTo(_request.ProviderId));
        }

        [Test]
        public void VerifyCacheRequestIdIsMapped()
        {
            Assert.That(_viewModel.CacheRequestId, Is.EqualTo(_request.CacheRequestId));
        }
    }
}
