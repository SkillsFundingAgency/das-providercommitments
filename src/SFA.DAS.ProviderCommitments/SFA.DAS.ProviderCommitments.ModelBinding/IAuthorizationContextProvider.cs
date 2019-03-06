using System;

namespace SFA.DAS.ProviderCommitments.ModelBinding
{
    public interface IAuthorizationContextProvider
    {
        IAuthorizationContext GetAuthorizationContext();
    }
}
