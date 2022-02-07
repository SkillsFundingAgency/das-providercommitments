using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class BlobFileTransferClient : IBlobFileTransferClient
    {
        private readonly ILogger<BlobFileTransferClient> _logger;
        private string _connectionString { get; }
        private string _containerName { get; set; }

        public BlobFileTransferClient(ILogger<BlobFileTransferClient> logger, BlobStorageSettings blobStorageSettings)
        {
            _logger = logger;
            _connectionString = blobStorageSettings.ConnectionString;
            _containerName = blobStorageSettings.BulkuploadContainer;
        }

        public async Task UploadFile(string fileContents, string path)
        {
            try
            {
                var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
                var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

                _logger.LogDebug($"Uploading {path} to blob storage {_containerName}");

                byte[] array = System.Text.Encoding.ASCII.GetBytes(fileContents);
                using (var stream = new MemoryStream(array))
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                _logger.LogDebug($"Uploaded {path} to blob storage {_containerName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file {path}");
                throw;
            }
        }

        public async Task<string> DownloadFile(string path)
        {
            var fileContent = string.Empty;

            try
            {
                var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
                var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

                _logger.LogDebug($"Downloading {path} from blob storage {_containerName}");

                using (var stream = new MemoryStream())
                {
                    await Download(path, stream);
                    using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }

                _logger.LogDebug($"Downloaded {path} from blob storage {_containerName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading {path} from blob storage {_containerName}");
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

                _logger.LogDebug($"Deleting {path} from blob storage {_containerName}");

                await blob.DeleteAsync();

                _logger.LogDebug($"Deleted {path} from blob storage {_containerName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {path} from blob storage {_containerName}");
                throw;
            }
        }

        private async Task Download(string path, MemoryStream stream)
        {
            var directory = await GetCloudBlobDirectory(GetBlobDirectoryName(path));
            var blob = directory.GetBlockBlobReference(GetBlobFileName(path));

            using (var memoryStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(memoryStream);

                memoryStream.Position = 0;
                memoryStream.CopyTo(stream);
                stream.Position = 0;
            }
        }

        private async Task<CloudBlobDirectory> GetCloudBlobDirectory(string path)
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_containerName);

            var directory = container.GetDirectoryReference(GetBlobDirectoryName(path));
            await container.CreateIfNotExistsAsync();

            return directory;
        }

        private string GetBlobFileName(string path)
        {
            return Path.GetFileName(path);
        }

        private string GetBlobDirectoryName(string path)
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
