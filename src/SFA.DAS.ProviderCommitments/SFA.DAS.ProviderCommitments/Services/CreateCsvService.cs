using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;

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
                        try
                        {
                            csvWriter.WriteRecords(results);
                            streamWriter.Flush();
                            memoryStream.Position = 0;
                            return memoryStream.ToArray();
                        }
                        catch (NullReferenceException)
                        {
                            throw new NullReferenceException();
                        }
                    }
                }
            }
        }
    }
}