namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public interface IBlobFileTransferClient
    {
        Task DeleteFile(string path);
        Task<string> DownloadFile(string path);
        Task UploadFile(string fileContents, string path);
    }
}