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
                  new BulkUploadAddDraftApprenticeshipRequest()
                  {
                      AgreementId = csvRecord.AgreementId,
                      CohortRef = csvRecord.CohortRef,
                      DateOfBirthAsString = csvRecord.DateOfBirth,
                      Email = csvRecord.EmailAddress,
                      EndDateAsString = csvRecord.EndDate,
                      LastName = csvRecord.FamilyName,
                      FirstName = csvRecord.GivenNames,
                      StartDateAsString = csvRecord.StartDate,
                      CourseCode = csvRecord.StdCode,
                      CostAsString = csvRecord.TotalPrice,
                      Uln = csvRecord.ULN,
                      ProviderRef = csvRecord.ProviderRef,
                      RowNumber = index + 1
                  }).ToList();

            return Task.FromResult(apiRequest);
        }
    }
}
