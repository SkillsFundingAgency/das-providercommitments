using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadStartViewModelToReviewRequestMapper : IMapper<FileUploadStartViewModel, FileUploadReviewRequest>
    {
        private readonly IBulkUploadFileParser _fileParser;
        private readonly ICacheService _cache;
        public FileUploadStartViewModelToReviewRequestMapper(IBulkUploadFileParser fileParser, ICacheService cache)
        {
            _fileParser = fileParser;
            _cache = cache;
        }

        public async Task<FileUploadReviewRequest> Map(FileUploadStartViewModel source)
        {
            var csvRecords = _fileParser.GetCsvRecords(source.ProviderId, source.Attachment);
            var cacheRequestId = await _cache.SetCache(csvRecords, nameof(FileUploadStartViewModelToReviewRequestMapper));

            return new FileUploadReviewRequest
            {
                ProviderId = source.ProviderId,
                CacheRequestId = cacheRequestId
            };
        }
    }
}
