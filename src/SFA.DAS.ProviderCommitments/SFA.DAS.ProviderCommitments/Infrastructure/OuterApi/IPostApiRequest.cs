using System;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi
{
    public interface IPostApiRequest
    {
        public string PostUrl { get; }
        public object Data { get; set; }
    }
}
