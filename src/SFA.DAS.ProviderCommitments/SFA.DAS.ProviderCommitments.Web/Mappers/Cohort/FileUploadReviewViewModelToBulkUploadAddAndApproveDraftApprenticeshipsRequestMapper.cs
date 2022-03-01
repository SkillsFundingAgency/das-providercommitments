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
            var csVRecrods = await _cacheService.GetFromCache<List<CsvRecord>>(source.CacheRequestId.ToString());
            await _cacheService.ClearCache(source.CacheRequestId.ToString());
            return new BulkUploadAddAndApproveDraftApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                BulkUploadAddAndApproveDraftApprenticeships = csVRecrods.Select((csv, i) => MapTo(i, csv, source.ProviderId))
            };
        }

        private BulkUploadAddDraftApprenticeshipRequest MapTo(int index, CsvRecord record, long providerId)
        {
            var legalEntityId = !string.IsNullOrWhiteSpace(record.AgreementId)
               ? _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId) : (long?)null;

            var cohortId = !string.IsNullOrWhiteSpace(record.CohortRef)
                ? _encodingService.Decode(record.CohortRef, EncodingType.CohortReference) : (long?)null;

            var bulkUploadApiRequest = record.MapToBulkUploadAddDraftApprenticeshipRequest(index + 1, providerId);
            bulkUploadApiRequest.CohortId = cohortId;
            bulkUploadApiRequest.LegalEntityId = legalEntityId;

            return bulkUploadApiRequest;
        }
    }
}
