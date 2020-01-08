using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class CreateCsvService : ICreateCsvService
    {
        public byte[] GenerateCsvContent<T>(IEnumerable<T> results)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.WriteRecords(results);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public byte[] GenerateCsvContent<T, TMap>(IEnumerable<T> results) where TMap : ClassMap<T>
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.RegisterClassMap<TMap>();
                        csvWriter.WriteRecords(results);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}