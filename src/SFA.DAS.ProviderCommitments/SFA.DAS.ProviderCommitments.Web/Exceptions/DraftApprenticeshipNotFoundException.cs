using System;

namespace SFA.DAS.ProviderCommitments.Web.Exceptions
{
    public class DraftApprenticeshipNotFoundException : Exception
    {
        public DraftApprenticeshipNotFoundException(string message = default, Exception ex = default) : base(message, ex)
        { }
    }
}
