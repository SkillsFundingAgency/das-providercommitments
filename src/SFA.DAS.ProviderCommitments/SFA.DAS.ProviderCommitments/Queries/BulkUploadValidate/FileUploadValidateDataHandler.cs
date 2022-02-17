using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataHandler : IRequestHandler<FileUploadValidateDataRequest, BulkUploadValidateApiResponse>
    {
        private ICommitmentsApiClient _client;
        private IModelMapper _modelMapper;
        private IBulkUploadFileParser _bulkUploadFileParser;

        public FileUploadValidateDataHandler(ICommitmentsApiClient client, IModelMapper modelMapper, IBulkUploadFileParser bulkUploadFileParser)
        {
            _client = client;
            _modelMapper = modelMapper;
            _bulkUploadFileParser = bulkUploadFileParser;
        }

        public async Task<BulkUploadValidateApiResponse> Handle(FileUploadValidateDataRequest request, CancellationToken cancellationToken)
        {
            request.CsvRecords = _bulkUploadFileParser.GetCsvRecords(request.ProviderId, request.Attachement);
            var apiRequest = await _modelMapper.Map<BulkUploadValidateApiRequest>(request);
            return await _client.ValidateBulkUploadRequest(request.ProviderId, apiRequest);
        }
    }
}
