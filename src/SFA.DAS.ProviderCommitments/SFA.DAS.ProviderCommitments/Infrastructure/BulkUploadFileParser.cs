using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class BulkUploadFileParser : IBulkUploadFileParser
    {
        private readonly ILogger<BulkUploadFileParser> _logger;

        public BulkUploadFileParser(ILogger<BulkUploadFileParser> logger)
        {
            _logger = logger;
        }

        public List<CsvRecord> GetCsvRecords(long providerId, IFormFile attachment)
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
                    var csvRecords = csvReader.GetRecords<CsvRecord>()
                        .ToList();

                    var emptyRecords = GetEmptyRecords(csvRecords);
                    if (emptyRecords.Count > 0)
                    {
                        emptyRecords.ForEach(item => csvRecords.Remove(item));
                    }

                    return csvRecords;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to process bulk upload file - ProviderId {providerId}");
                throw exc;
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