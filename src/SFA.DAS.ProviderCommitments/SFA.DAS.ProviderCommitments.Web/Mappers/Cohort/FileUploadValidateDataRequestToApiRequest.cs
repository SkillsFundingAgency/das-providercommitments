using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadValidateDataRequestToApiRequest : FileUploadMapperBase, IMapper<FileUploadValidateDataRequest, BulkUploadValidateApimRequest>
    {
        private readonly Authentication.IAuthenticationService _authenticationService;

        public FileUploadValidateDataRequestToApiRequest(IEncodingService encodingService, IOuterApiService outerApiService, Authentication.IAuthenticationService authenticationService) 
            :base(encodingService, outerApiService)
        {
            _authenticationService = authenticationService;
        }

        public Task<BulkUploadValidateApimRequest> Map(FileUploadValidateDataRequest source)
        {
            var apiRequest = new BulkUploadValidateApimRequest();
            apiRequest.ProviderId = source.ProviderId;
            apiRequest.RplDataExtended = source.RplDataExtended;
            apiRequest.CsvRecords = ConvertToBulkUploadApiRequest(source.CsvRecords, source.ProviderId);

            var userinfo = new ApimUserInfo()
            {
                UserDisplayName = _authenticationService.UserName,
                UserEmail = _authenticationService.UserEmail,
                UserId = _authenticationService.UserId,
            };

            apiRequest.UserInfo = userinfo;
            
            return Task.FromResult(apiRequest);
        }
    }
}
