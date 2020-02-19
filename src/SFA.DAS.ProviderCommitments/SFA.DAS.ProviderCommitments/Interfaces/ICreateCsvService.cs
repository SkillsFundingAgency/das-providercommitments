using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ProviderCommitments.Services
{
    public interface ICreateCsvService
    {
        byte[] GenerateCsvContent<T>(IEnumerable<T> results);
    }
}