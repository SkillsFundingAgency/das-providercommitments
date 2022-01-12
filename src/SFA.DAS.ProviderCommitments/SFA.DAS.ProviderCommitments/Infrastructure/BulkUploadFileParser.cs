using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                    return csvReader.GetRecords<CsvRecord>()
                             .ToList();
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to process bulk upload file - ProviderId {providerId}");
                throw exc;
            }
        }
    }
}
