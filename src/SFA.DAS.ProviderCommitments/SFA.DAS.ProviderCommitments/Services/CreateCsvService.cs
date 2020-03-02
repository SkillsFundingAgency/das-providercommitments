using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class CreateCsvService : ICreateCsvService
    {
        private MemoryStream _memoryStream;
        private StreamWriter _streamWriter;
        private CsvWriter _csvWriter;

        public MemoryStream GenerateCsvContent<T>(IEnumerable<T> results, bool hasHeader)
        {
            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream);
            _csvWriter = new CsvWriter(_streamWriter, new CsvHelper.Configuration.Configuration
            {
                HasHeaderRecord = hasHeader
            });

            _csvWriter.WriteRecords(results);
            _streamWriter.Flush();
            _memoryStream.Position = 0;

            return _memoryStream;
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
            _csvWriter.Dispose();
            _streamWriter.Dispose();
        }
    }
}