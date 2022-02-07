using SFA.DAS.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry.ToRemove
{
    public class CommitmentApiClient2 : CommitmentsV2.Api.Client.CommitmentsApiClient
    {
        private readonly IRestHttpClient _client;

        public CommitmentApiClient2(IRestHttpClient client) :base(client)
        {
            _client = client;
        }


        public Task ValidateBulkUploadRequest(long providerId, BulkUploadValidateRequest request, CancellationToken cancellationToken = default)
        {
            return _client.PostAsJson($"api/{providerId}/bulkupload/validate", request, cancellationToken);
        }

    }
}
