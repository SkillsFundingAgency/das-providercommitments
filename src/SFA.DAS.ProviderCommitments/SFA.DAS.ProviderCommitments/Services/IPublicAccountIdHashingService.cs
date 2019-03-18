using System.Runtime.CompilerServices;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderCommitments.Services
{
    /// <summary>
    ///     Separate interface so that we can distinguish it for IoC injection
    /// </summary>
    public interface IPublicAccountIdHashingService : IHashingService
    {
       
    }
}