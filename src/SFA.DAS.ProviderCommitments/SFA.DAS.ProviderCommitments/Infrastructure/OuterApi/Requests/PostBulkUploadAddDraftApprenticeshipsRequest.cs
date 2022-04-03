namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostBulkUploadAddDraftApprenticeshipsRequest : IPostApiRequest
    {
        public string PostUrl => "BulkUpload/add";

        public object Data { get; set; }
        public PostBulkUploadAddDraftApprenticeshipsRequest(BulkUploadAddDraftApprenticeshipsRequest data)
        {
            Data = data;
        }
    }
}
