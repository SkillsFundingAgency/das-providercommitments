using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadValidateDataRequestToApiRequest : IMapper<FileUploadValidateDataRequest, BulkUploadValidateApiRequest>
    {
        public Task<BulkUploadValidateApiRequest> Map(FileUploadValidateDataRequest source)
        {
            var apiRequest = new BulkUploadValidateApiRequest();
            apiRequest.ProviderId = source.ProviderId;

            apiRequest.CsvRecords = source.CsvRecords.Select((csvRecord, index) =>
                csvRecord.MapToBulkUploadAddDraftApprenticeshipRequest(index+ 1, source.ProviderId)).ToList();

            return Task.FromResult(apiRequest);
        }
    }
}
