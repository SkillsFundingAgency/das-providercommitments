using MediatR;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataHandler : IRequestHandler<FileUploadValidateDataRequest>
    {
        private IOuterApiService _client;
        private IModelMapper _modelMapper;
        private IBulkUploadFileParser _bulkUploadFileParser;

        public FileUploadValidateDataHandler(IOuterApiService client, IModelMapper modelMapper, IBulkUploadFileParser bulkUploadFileParser)
        {
            _client = client;
            _modelMapper = modelMapper;
            _bulkUploadFileParser = bulkUploadFileParser;
        }

        public async Task<Unit> Handle(FileUploadValidateDataRequest request, CancellationToken cancellationToken)
        {
            request.CsvRecords = _bulkUploadFileParser.GetCsvRecords(request.ProviderId, request.Attachment);
            var apiRequest = await _modelMapper.Map<BulkUploadValidateApiRequest>(request);
            await _client.ValidateBulkUploadRequest(apiRequest);
            return Unit.Value;
        }
    }
}
