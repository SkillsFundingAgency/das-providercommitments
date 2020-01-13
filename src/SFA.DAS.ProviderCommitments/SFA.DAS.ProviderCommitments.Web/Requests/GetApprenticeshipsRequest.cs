namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class GetApprenticeshipsRequest
    {
        public uint ProviderId { get; set; }
        public int PageNumber { get; set; }
        public int PageItemCount { get; set; }
    }
}
