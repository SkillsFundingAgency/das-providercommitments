using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper : FileUploadMapperBase, IMapper<FileUploadReviewViewModel, BulkUploadAddDraftApprenticeshipsRequest>
    {
        private readonly ICacheService _cacheService;
        private readonly IAuthorizationService _authorizationService;

        public FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper(ICacheService cacheService, IEncodingService encodingService, IOuterApiService outerApiService,
            IAuthorizationService authorizationService)
            : base(encodingService, outerApiService)
        {
            _cacheService = cacheService;
            _authorizationService = authorizationService;
        }

        public async Task<BulkUploadAddDraftApprenticeshipsRequest> Map(FileUploadReviewViewModel source)
        {
            var cacheModel = await _cacheService.GetFromCache<FileUploadCacheModel>(source.CacheRequestId.ToString());
            var csVRecords = cacheModel.CsvRecords;
            var fileUploadLogId = cacheModel.FileUploadLogId;
            await _cacheService.ClearCache(source.CacheRequestId.ToString(),nameof(FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper));
            return new BulkUploadAddDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                FileUploadLogId = fileUploadLogId,
                BulkUploadDraftApprenticeships = ConvertToBulkUploadApiRequest(csVRecords, source.ProviderId)
            };
        }
    }
}
