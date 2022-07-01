using System;

namespace SFA.DAS.ProviderCommitments.Web.Services.Cache
{
    public class CacheItemNotFoundException : Exception
    {
        public CacheItemNotFoundException()
        {
        }

        public CacheItemNotFoundException(string message) : base(message)
        {
        }

        public CacheItemNotFoundException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
