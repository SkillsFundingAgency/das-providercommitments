using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadValidateDataRequestToApiRequest : FileUploadMapperBase, IMapper<FileUploadValidateDataRequest, BulkUploadValidateApimRequest>
    {
        public Task<BulkUploadValidateApimRequest> Map(FileUploadValidateDataRequest source)
        {
            var apiRequest = new BulkUploadValidateApimRequest();
            apiRequest.ProviderId = source.ProviderId;

            apiRequest.CsvRecords = ConvertToBulkUploadApiRequest(source.CsvRecords, source.ProviderId);
            return Task.FromResult(apiRequest);
        }
    }
}
