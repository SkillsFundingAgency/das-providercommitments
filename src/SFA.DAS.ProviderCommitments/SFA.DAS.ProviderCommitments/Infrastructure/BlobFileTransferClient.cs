using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class BlobFileTransferClient : IBlobFileTransferClient
    {
        private readonly ILogger<BlobFileTransferClient> _logger;
        private string ConnectionString { get; }
        private string ContainerName { get; set; }

        public BlobFileTransferClient(ILogger<BlobFileTransferClient> logger, BlobStorageSettings blobStorageSettings)
        {
            _logger = logger;
            ConnectionString = blobStorageSettings.ConnectionString;
            ContainerName = blobStorageSettings.BulkuploadContainer;
        }

        public async Task UploadFile(string fileContents, string path)
        {
            try
            {
                var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
                var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

                _logger.LogDebug("Uploading {path} to blob storage {ContainerName}", path, ContainerName);

                var array = System.Text.Encoding.ASCII.GetBytes(fileContents);
                using (var stream = new MemoryStream(array))
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                _logger.LogDebug("Uploaded {path} to blob storage {ContainerName}", path, ContainerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {path}", path);
                throw;
            }
        }

        public async Task<string> DownloadFile(string path)
        {
            string fileContent;

            try
            {
                _logger.LogDebug("Downloading {path} from blob storage {ContainerName}", path, ContainerName);

                using (var stream = new MemoryStream())
                {
                    await Download(path, stream);
                    using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        fileContent = await reader.ReadToEndAsync();
                    }
                }

                _logger.LogDebug("Downloaded {path} from blob storage {ContainerName}", path, ContainerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading {path} from blob storage {ContainerName}", path, ContainerName);
                throw;
            }

            return fileContent;
        }

        public async Task DeleteFile(string path)
        {
            try
            {
                var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
                var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

                _logger.LogDebug("Deleting {path} from blob storage {ContainerName}", path, ContainerName);

                await blob.DeleteAsync();

                _logger.LogDebug("Deleted {path} from blob storage {ContainerName}", path, ContainerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {path} from blob storage {ContainerName}", path, ContainerName);
                throw;
            }
        }

        private async Task Download(string path, Stream stream)
        {
            var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
            var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(stream);
                stream.Position = 0;
            }
        }

        private async Task<CloudBlobDirectory> GetCloudBlobDirectory(string path)
        {
            var account = CloudStorageAccount.Parse(ConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(ContainerName);

            var directory = container.GetDirectoryReference(GetBlobDirectoryName(path));
            await container.CreateIfNotExistsAsync();

            return directory;
        }

        private static string GetBlobFileName(string path)
        {
            return Path.GetFileName(path);
        }

        private static string GetBlobDirectoryName(string path)
        {
            var directoryName = Path.GetDirectoryName(path);

            directoryName = !string.IsNullOrEmpty(directoryName)
                ? directoryName.Replace('\\', '/').TrimStart('/')
                : path;

            return directoryName.EndsWith('/')
                ? directoryName
                : directoryName += '/';
        }
    }
}
