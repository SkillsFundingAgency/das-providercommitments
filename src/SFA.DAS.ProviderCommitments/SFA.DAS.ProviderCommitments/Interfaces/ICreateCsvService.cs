using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace SFA.DAS.ProviderCommitments.Services
{
    public interface ICreateCsvService
    {
        MemoryStream GenerateCsvContent<T>(IEnumerable<T> results, bool hasHeader);
        void Dispose();
    }
}