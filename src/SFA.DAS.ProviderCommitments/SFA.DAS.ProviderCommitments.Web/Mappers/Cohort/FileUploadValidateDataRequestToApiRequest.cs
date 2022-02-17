using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadValidateDataRequestToApiRequest : IMapper<FileUploadValidateDataRequest, BulkUploadValidateApiRequest>
    {
        public Task<BulkUploadValidateApiRequest> Map(FileUploadValidateDataRequest source)
        {
            int counter = 1;
            var apiRequest = new BulkUploadValidateApiRequest();
            apiRequest.ProviderId = source.ProviderId;
            var records = new List<CsvRecord>();
            foreach (var sourceCsvRecrod in source.CsvRecords)
            {
                var apiCsvRecord = new CsvRecord();
                apiCsvRecord.AgreementId = sourceCsvRecrod.AgreementId;
                apiCsvRecord.CohortRef = sourceCsvRecrod.CohortRef;
                apiCsvRecord.DateOfBirth = sourceCsvRecrod.DateOfBirth;
                apiCsvRecord.EmailAddress = sourceCsvRecrod.EmailAddress;
                apiCsvRecord.EndDate = sourceCsvRecrod.EndDate;
                apiCsvRecord.FamilyName = sourceCsvRecrod.FamilyName;
                apiCsvRecord.GivenNames = sourceCsvRecrod.GivenNames;
                apiCsvRecord.StartDate = sourceCsvRecrod.StartDate;
                apiCsvRecord.StdCode = sourceCsvRecrod.StdCode;
                apiCsvRecord.TotalPrice = sourceCsvRecrod.TotalPrice;
                apiCsvRecord.ULN = sourceCsvRecrod.ULN;
                apiCsvRecord.ProviderRef = sourceCsvRecrod.ProviderRef;
                apiCsvRecord.RowNumber = counter++;
                records.Add(apiCsvRecord);
            }

            apiRequest.CsvRecords = records;
            return Task.FromResult(apiRequest);
        }
    }
}
