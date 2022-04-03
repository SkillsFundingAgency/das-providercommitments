namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostBulkUploadAddAndApproveDraftApprenticeshipsRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/addandapprove";

        public object Data { get; set; }
        public PostBulkUploadAddAndApproveDraftApprenticeshipsRequest(BulkUploadAddAndApproveDraftApprenticeshipsRequest data)
        {
            Data = data;
        }
    }
}
