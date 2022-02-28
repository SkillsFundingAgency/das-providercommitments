using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
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
                  new CsvRecord()
                  {
                      AgreementId = csvRecord.AgreementId,
                      CohortRef = csvRecord.CohortRef,
                      DateOfBirth = csvRecord.DateOfBirth,
                      EmailAddress = csvRecord.EmailAddress,
                      EndDate = csvRecord.EndDate,
                      FamilyName = csvRecord.FamilyName,
                      GivenNames = csvRecord.GivenNames,
                      StartDate = csvRecord.StartDate,
                      StdCode = csvRecord.StdCode,
                      TotalPrice = csvRecord.TotalPrice,
                      ULN = csvRecord.ULN,
                      ProviderRef = csvRecord.ProviderRef,
                      RowNumber = index +1
                  }).ToList();

            return Task.FromResult(apiRequest);
        }
    }
}
