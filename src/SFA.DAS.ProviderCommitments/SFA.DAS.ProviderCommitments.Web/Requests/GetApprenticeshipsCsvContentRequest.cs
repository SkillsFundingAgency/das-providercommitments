using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class GetApprenticeshipsCsvContentRequest
    {
        public long ProviderId { get; set; }
        public ApprenticesFilterModel FilterModel { get; set; }
    }
}
