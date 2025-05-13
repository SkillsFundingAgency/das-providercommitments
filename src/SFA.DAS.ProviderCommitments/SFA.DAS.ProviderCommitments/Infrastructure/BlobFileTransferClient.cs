using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Infrastructure;

public class BlobFileTransferClient : IBlobFileTransferClient{
    private readonly ILogger<BlobFileTransferClient> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;

    public BlobFileTransferClient(ILogger<BlobFileTransferClient> logger, BlobStorageSettings blobStorageSettings)
    {
        _logger = logger;
        _blobServiceClient = new BlobServiceClient(blobStorageSettings.ConnectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(blobStorageSettings.BulkuploadContainer);
    }

    public async Task UploadFile(string fileContents, string path)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(path);
            _logger.LogDebug("Uploading {path} to blob storage {ContainerName}", path, _containerClient.Name);

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents)))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
                
            _logger.LogDebug("Uploaded {path} to blob storage {ContainerName}", path, _containerClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {path}", path);
            throw;
        }
    }

    public async Task<string> DownloadFile(string path)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(path);
            _logger.LogDebug("Downloading {path} from blob storage {ContainerName}", path, _containerClient.Name);

            var response = await blobClient.DownloadContentAsync();
            string content = response.Value.Content.ToString();
                
            _logger.LogDebug("Downloaded {path} from blob storage {ContainerName}", path, _containerClient.Name);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading {path}", path);
            throw;
        }
    }

    public async Task DeleteFile(string path)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(path);
            _logger.LogDebug("Deleting {path} from blob storage {ContainerName}", path, _containerClient.Name);

            await blobClient.DeleteIfExistsAsync();

            _logger.LogDebug("Deleted {path} from blob storage {ContainerName}", path, _containerClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting {path} from blob storage {ContainerName}", path, _containerClient.Name);
            throw;
        }
    }
}