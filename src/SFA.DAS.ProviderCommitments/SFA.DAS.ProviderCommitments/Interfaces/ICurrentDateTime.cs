using System;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface ICurrentDateTime
    {
        DateTime Now { get; }
    }
}