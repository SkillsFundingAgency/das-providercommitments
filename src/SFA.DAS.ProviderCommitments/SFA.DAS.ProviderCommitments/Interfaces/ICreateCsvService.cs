using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface ICreateCsvService
    {
        byte[] GenerateCsvContent<T>(IEnumerable<T> results);
    }
}