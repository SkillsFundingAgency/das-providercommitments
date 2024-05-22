using System;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataHandler : IRequestHandler<FileUploadValidateDataRequest, FileUploadValidateDataResponse>
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

        public async Task<FileUploadValidateDataResponse> Handle(FileUploadValidateDataRequest request, CancellationToken cancellationToken)
        {
            request.CsvRecords = _bulkUploadFileParser.GetCsvRecords(request.ProviderId, request.Attachment);
           
            var apiRequest = await _modelMapper.Map<BulkUploadValidateApimRequest>(request);
            apiRequest.FileUploadLogId = await _client.CreateFileUploadLog(request.ProviderId, request.Attachment, request.CsvRecords);
            apiRequest.RplDataExtended = await _authorizationService.IsAuthorizedAsync(ProviderFeature.RplExtended);
            try
            {
                await _client.ValidateBulkUploadRequest(apiRequest);
                return new FileUploadValidateDataResponse
                {
                    LogId = apiRequest.FileUploadLogId
                };
            }
            catch (CommitmentsApiBulkUploadModelException ex)
            {
                await _client.AddValidationMessagesToFileUploadLog(request.ProviderId, apiRequest.FileUploadLogId, ex.Errors);
                throw;
            }
            catch (Exception ex)
            {
                await _client.AddUnhandledExceptionToFileUploadLog(request.ProviderId, apiRequest.FileUploadLogId, ex.Message);
                throw;
            }
        }
    }
}
