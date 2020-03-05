using System.Collections.Generic;
using System.IO;
using CsvHelper;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;

namespace SFA.DAS.ProviderCommitments.Services
{
    public class CreateCsvService : ICreateCsvService
    {
        public MemoryStream GenerateCsvContent<T>(IEnumerable<T> results, bool hasHeader)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        if (hasHeader)
                        {
                            csvWriter.WriteHeader<T>();
                        }

                        csvWriter.WriteRecords(results);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }
        }

        public void Dispose()
        {
           
        }
    }
}