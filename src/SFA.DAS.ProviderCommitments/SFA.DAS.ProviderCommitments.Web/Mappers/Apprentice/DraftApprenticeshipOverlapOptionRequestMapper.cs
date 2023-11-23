using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;
using System;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DraftApprenticeshipOverlapOptionRequestMapper : IMapper<ChangeOfEmployerOverlapAlertViewModel, DraftApprenticeshipOverlapOptionRequest>
    {
        private readonly ICacheStorageService _cacheStorage;


        public DraftApprenticeshipOverlapOptionRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<DraftApprenticeshipOverlapOptionRequest> Map(ChangeOfEmployerOverlapAlertViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new DraftApprenticeshipOverlapOptionRequest
            {
                DraftApprenticeshipHashedId = "",
                CohortReference = cacheItem.CohortReference,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId
            };
        }

    }
}
