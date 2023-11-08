using System;
using MediatR;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;

namespace SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate
{
    public class FileUploadValidateDataHandler : IRequestHandler<FileUploadValidateDataRequest, FileUploadValidateDataResponse>
    {
        private IOuterApiService _client;
        private IModelMapper _modelMapper;
        private IBulkUploadFileParser _bulkUploadFileParser;
        private readonly IAuthorizationService _authorizationService;

        public FileUploadValidateDataHandler(IOuterApiService client, IModelMapper modelMapper, IBulkUploadFileParser bulkUploadFileParser, SFA.DAS.Authorization.Services.IAuthorizationService authorizationService)
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
            apiRequest.RplDataExtended = _authorizationService.IsAuthorized(ProviderFeature.RplExtended);
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
