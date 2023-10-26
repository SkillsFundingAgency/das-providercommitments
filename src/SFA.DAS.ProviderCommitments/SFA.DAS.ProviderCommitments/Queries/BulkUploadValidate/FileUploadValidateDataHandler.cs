using MediatR;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataHandler : IRequestHandler<FileUploadValidateDataRequest>
    {
        private readonly IOuterApiService _client;
        private readonly IModelMapper _modelMapper;
        private readonly IBulkUploadFileParser _bulkUploadFileParser;
        private readonly IAuthorizationService _authorizationService;

        public FileUploadValidateDataHandler(IOuterApiService client, IModelMapper modelMapper, IBulkUploadFileParser bulkUploadFileParser, IAuthorizationService authorizationService)
        {
            _client = client;
            _modelMapper = modelMapper;
            _bulkUploadFileParser = bulkUploadFileParser;
            _authorizationService = authorizationService;
        }

        public async Task Handle(FileUploadValidateDataRequest request, CancellationToken cancellationToken)
        {
            request.CsvRecords = _bulkUploadFileParser.GetCsvRecords(request.ProviderId, request.Attachment);
           
            var apiRequest = await _modelMapper.Map<BulkUploadValidateApimRequest>(request);
            apiRequest.RplDataExtended = _authorizationService.IsAuthorized(ProviderFeature.RplExtended);
            
            await _client.ValidateBulkUploadRequest(apiRequest);
        }
    }
}
