using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Features;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class BulkUploadFileParser : IBulkUploadFileParser
    {
        private readonly ILogger<BulkUploadFileParser> _logger;
        private readonly IAuthorizationService _authorizationService;

        public BulkUploadFileParser(ILogger<BulkUploadFileParser> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public List<CsvRecord> GetCsvRecords(long providerId, IFormFile attachment)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                    BadDataFound = args => throw new Exception("Bad data found"),
                    HeaderValidated = ValidateHeader,
                    MissingFieldFound = MissingFieldFound,
                };

                var fileContent = new StreamReader(attachment.OpenReadStream()).ReadToEnd();
                using var tr = new StringReader(fileContent);
                var csvReader = new CsvReader(tr, config);

                csvReader.Context.RegisterClassMap<CsvRecordMap>();
                var csvRecords = csvReader.GetRecords<CsvRecord>()
                         .ToList();

                var emptyRecords = GetEmptyRecords(csvRecords);
                if (emptyRecords.Count > 0)
                {
                    emptyRecords.ForEach(item => csvRecords.Remove(item));
                }

                return csvRecords;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to process bulk upload file - ProviderId {providerId}");
                throw exc;
            }
        }

        private void ValidateHeader(HeaderValidatedArgs args)
        {
            var missingHeaders = args.InvalidHeaders.SelectMany(h => h.Names);
            var missingRequiredHeaders = missingHeaders;
            if (!_authorizationService.IsAuthorized(ProviderFeature.RecognitionOfPriorLearning))
            {
                missingRequiredHeaders = missingHeaders.Except(BulkUploadFileRequirements.OptionalHeaders);
            }

            if (missingRequiredHeaders.Any())
            {
                // Default validation
                ConfigurationFunctions.HeaderValidated(args);
            }
        }

        public void MissingFieldFound(MissingFieldFoundArgs args)
        {
            var missingRequiredFields = args.HeaderNames.Except(BulkUploadFileRequirements.OptionalHeaders);

            if (missingRequiredFields.Any())
            {
                // Default validation
                ConfigurationFunctions.MissingFieldFound(args);
            }
        }

        public static List<CsvRecord> GetEmptyRecords(List<CsvRecord> csvRecords)
        {
            return (csvRecords.Where(csvRecord => string.IsNullOrEmpty(csvRecord.CohortRef) &&
                    string.IsNullOrEmpty(csvRecord.AgreementId) &&
                    string.IsNullOrEmpty(csvRecord.ULN) &&
                    string.IsNullOrEmpty(csvRecord.FamilyName) &&
                    string.IsNullOrEmpty(csvRecord.GivenNames) &&
                    string.IsNullOrEmpty(csvRecord.DateOfBirth) &&
                    string.IsNullOrEmpty(csvRecord.EmailAddress) &&
                    string.IsNullOrEmpty(csvRecord.StdCode) &&
                    string.IsNullOrEmpty(csvRecord.StartDate) &&
                    string.IsNullOrEmpty(csvRecord.EndDate) &&
                    string.IsNullOrEmpty(csvRecord.TotalPrice) &&
                    string.IsNullOrEmpty(csvRecord.EPAOrgID) &&
                    string.IsNullOrEmpty(csvRecord.ProviderRef))
                    ).ToList();
        }
    }
}