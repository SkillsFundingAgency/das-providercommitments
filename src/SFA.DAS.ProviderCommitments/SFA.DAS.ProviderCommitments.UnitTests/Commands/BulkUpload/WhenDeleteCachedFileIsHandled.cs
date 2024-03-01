using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.UnitTests.Commands.BulkUpload;

public class WhenDeleteCachedFileIsHandled
{
    WhenDeleteCachedFileIsHandledFixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _fixture = new WhenDeleteCachedFileIsHandledFixture();
    }

    [Test]
    public async Task ThenCachedFileIsDeleted()
    {
        await _fixture.Handle();
        _fixture.VerifyCachedFileIsDeleted();
    }

    private class WhenDeleteCachedFileIsHandledFixture
    {
        private readonly DeleteCachedFileHandler _handler;
        private readonly Mock<ICacheService> _cacheService;
        private readonly DeleteCachedFileCommand _command;

        public WhenDeleteCachedFileIsHandledFixture()
        {
            var fixture = new Fixture();
            _command = fixture.Create<DeleteCachedFileCommand>();
            _cacheService = new Mock<ICacheService>();

            _handler = new DeleteCachedFileHandler(_cacheService.Object);
        }

        public async Task Handle()
        {
            await _handler.Handle(_command, CancellationToken.None);
        }

        internal void VerifyCachedFileIsDeleted()
        {
            _cacheService.Verify(x => x.ClearCache(_command.CachedRequestId.ToString(), It.IsAny<string>()), Times.Once);
        }
    }
}