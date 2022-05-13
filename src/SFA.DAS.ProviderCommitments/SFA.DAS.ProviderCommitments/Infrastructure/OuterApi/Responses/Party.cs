using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses
{
    [Flags]
    public enum Party : short
    {
        None = 0,
        Employer = 1,
        Provider = 2,
        TransferSender = 4
    }
}
