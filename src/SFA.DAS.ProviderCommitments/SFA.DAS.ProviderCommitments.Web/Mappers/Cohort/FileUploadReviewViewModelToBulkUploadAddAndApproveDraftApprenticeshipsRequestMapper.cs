﻿using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper : IMapper<FileUploadReviewViewModel, BulkUploadAddAndApproveDraftApprenticeshipsRequest>
    {
        private readonly ICacheService _cacheService;
        private readonly IEncodingService _encodingService;
        public FileUploadReviewViewModelToBulkUploadAddAndApproveDraftApprenticeshipsRequestMapper(ICacheService cacheService, IEncodingService encodingService)
        {
            _cacheService = cacheService;
            _encodingService = encodingService;
        }

        public async Task<BulkUploadAddAndApproveDraftApprenticeshipsRequest> Map(FileUploadReviewViewModel source)
        {
            var csVRecords = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            await _cacheService.ClearCache(source.CacheRequestId.ToString());
            return new BulkUploadAddAndApproveDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                BulkUploadAddAndApproveDraftApprenticeships = csVRecords.Select((csv, i) => 
                    csv.MapToBulkUploadAddDraftApprenticeshipRequest(i + 1, source.ProviderId,_encodingService))
            };
        }
    }
}
