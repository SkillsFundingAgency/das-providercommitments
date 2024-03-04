using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper : FileUploadMapperBase, IMapper<FileUploadReviewViewModel, BulkUploadAddAndApproveDraftApprenticeshipsRequest>
    {
        private readonly ICacheService _cacheService;

        public FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper(ICacheService cacheService, IEncodingService encodingService, IOuterApiService outerApiService)
            : base(encodingService, outerApiService)
        {
            _cacheService = cacheService;
        }

        public async Task<BulkUploadAddAndApproveDraftApprenticeshipsRequest> Map(FileUploadReviewViewModel source)
        {
            var cacheModel = await _cacheService.GetFromCache<FileUploadCacheModel>(source.CacheRequestId.ToString());
            var csvRecords = cacheModel.CsvRecords;
            var fileUploadLogId = cacheModel.FileUploadLogId;

            await _cacheService.ClearCache(source.CacheRequestId.ToString(), nameof(FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper));
            return new BulkUploadAddAndApproveDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                FileUploadLogId = fileUploadLogId,
                BulkUploadAddAndApproveDraftApprenticeships = ConvertToBulkUploadApiRequest(csvRecords, source.ProviderId, false)
            };
        }
    }
}
