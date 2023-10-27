using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.Authorization.Services;

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
            var csVRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            await _cacheService.ClearCache(source.CacheRequestId.ToString(),nameof(FileUploadReviewViewModelToBulkUploadAddDraftApprenticeshipsRequestMapper));
            var rplExtended = await _authorizationService.IsAuthorizedAsync(Features.ProviderFeature.RplExtended);
            return new BulkUploadAddDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                RplDataExtended = rplExtended,
                BulkUploadDraftApprenticeships = ConvertToBulkUploadApiRequest(csVRecords, source.ProviderId, rplExtended)
            };
        }
    }
}
