using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.Encoding;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public sealed class BulkUploadFileParser : IBulkUploadFileParser
    {
        private IEncodingService _encodingService;

        private readonly ILogger<BulkUploadFileParser> _logger;

        public BulkUploadFileParser(IEncodingService encodingService, ILogger<BulkUploadFileParser> logger)
        {
            _encodingService = encodingService;
            _logger = logger;
        }

        public BulkUploadAddDraftApprenticeshipsRequest CreateApiRequest(long providerId, IFormFile attachment)
        {
            try
            {
                var fileContent = new StreamReader(attachment.OpenReadStream()).ReadToEnd();
                using (var tr = new StringReader(fileContent))
                {
                    var csvReader = new CsvReader(tr);
                    csvReader.Configuration.HasHeaderRecord = true;
                    csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();
                    csvReader.Configuration.BadDataFound = cont => throw new Exception("Bad data found");
                    csvReader.Configuration.RegisterClassMap<CsvRecordMap>();

                    return new BulkUploadAddDraftApprenticeshipsRequest
                    {
                        DraftApprenticeshipRequest = csvReader.GetRecords<CsvRecord>()
                            .ToList()
                            .Select(record => MapTo(record))
                    };
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to process bulk upload file - ProviderId {providerId}");
                throw exc;
            }
        }

        private BulkUploadAddDraftApprenticeshipRequest MapTo(CsvRecord record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");
            var learnerStartDate = GetValidDate(record.StartDate, "yyyy-MM");
            var learnerEndDate = GetValidDate(record.EndDate, "yyyy-MM");

            return new BulkUploadAddDraftApprenticeshipRequest
            {
                Uln = record.ULN,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirth = dateOfBirth,
                Cost =  int.Parse(record.TotalPrice),
                //ProviderRef = record.ProviderRef,
                StartDate = learnerStartDate,
                EndDate = learnerEndDate,
                CourseCode = record.StdCode,
                LegalEntityId = _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId),
                CohortId = _encodingService.Decode(record.CohortRef, EncodingType.CohortReference),
                Email = record.EmailAddress
            };
        }

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }
}
