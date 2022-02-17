//using SFA.DAS.CommitmentsV2.Api.Types.Requests;
//using SFA.DAS.CommitmentsV2.Api.Types.Responses;
//using SFA.DAS.Http;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry.ToRemove
//{
//    public class CommitmentApiClient2 : CommitmentsV2.Api.Client.CommitmentsApiClient
//    {
//        private readonly IRestHttpClient _client;

//        public CommitmentApiClient2(IRestHttpClient client) :base(client)
//        {
//            _client = client;
//        }


//        public Task<BulkUploadValidateApiResponse> ValidateBulkUploadRequest(long providerId, BulkUploadValidateApiRequest request, CancellationToken cancellationToken = default)
//        {
//            return _client.PostAsJson<BulkUploadValidateApiRequest, BulkUploadValidateApiResponse>($"api/{providerId}/bulkupload/validate", request, cancellationToken);
//        }

//    }
//}
