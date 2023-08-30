using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class FileUploadUpdateLogRequest : ApimSaveDataRequest
    {
        public long ProviderId { get; set; }
        public long LogId { get; set; }
    }
}
