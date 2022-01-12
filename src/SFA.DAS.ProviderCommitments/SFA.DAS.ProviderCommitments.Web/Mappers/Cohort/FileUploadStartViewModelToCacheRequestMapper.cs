using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadStartViewModelToCacheRequestMapper : IMapper<FileUploadStartViewModel, FileUploadCacheRequest>
    {
        private readonly IBulkUploadFileParser _fileParser;
        private readonly ICacheService _cache;
        public FileUploadStartViewModelToCacheRequestMapper(IBulkUploadFileParser fileParser, ICacheService cache)
        {
            _fileParser = fileParser;
            _cache = cache;
        }

        public async Task<FileUploadCacheRequest> Map(FileUploadStartViewModel source)
        {
            var csvRecords = _fileParser.GetCsvRecords(source.ProviderId, source.Attachment);
            var cacheRequestId = await _cache.SetCache(csvRecords);

            return new FileUploadCacheRequest
            {
                ProviderId = source.ProviderId,
                CacheRequestId = cacheRequestId
            };
        }
    }
}
