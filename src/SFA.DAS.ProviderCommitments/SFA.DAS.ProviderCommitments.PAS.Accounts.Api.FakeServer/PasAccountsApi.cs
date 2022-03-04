using System;
using WireMock.Server;

namespace SFA.DAS.ProviderCommitments.PAS.Accounts.Api.FakeServer
{
    public class PasAccountsApi : IDisposable
    {
        private readonly WireMockServer _server;

        private bool _isDisposed;

        public PasAccountsApi(WireMockServer server)
        {
            _server = server;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                if (_server != null && _server.IsStarted)
                {
                    _server.Stop();
                }

                _server?.Dispose();
            }

            _isDisposed = true;
        }
    }
}