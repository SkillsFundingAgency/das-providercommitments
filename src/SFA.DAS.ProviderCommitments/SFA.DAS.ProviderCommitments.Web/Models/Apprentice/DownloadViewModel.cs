using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class DownloadViewModel
    {
        public string Name { get; set; }
        public string ContentType => "application/octet-stream";
        public PostApprenticeshipsCSVRequest Request { get; set; }
        public Stream Content { get; set; }
    }
}
