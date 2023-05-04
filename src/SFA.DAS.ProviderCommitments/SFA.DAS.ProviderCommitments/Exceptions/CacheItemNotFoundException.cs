using System;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Exceptions
{
    public class CacheItemNotFoundException<T> : Exception
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
