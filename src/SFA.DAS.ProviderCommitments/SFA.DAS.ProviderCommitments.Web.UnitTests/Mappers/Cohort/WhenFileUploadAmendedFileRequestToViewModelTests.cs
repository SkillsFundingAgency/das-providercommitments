using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

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
            Assert.AreEqual(_request.ProviderId, _viewModel.ProviderId);
        }

        [Test]
        public void VerifyCacheRequestIdIsMapped()
        {
            Assert.AreEqual(_request.CacheRequestId, _viewModel.CacheRequestId);
        }
    }
}
