using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class CreateChangeOfPartyRequestMapper : IMapper<ConfirmViewModel, CreateChangeOfPartyRequestRequest>
    {
        private readonly ICacheStorageService _cacheStorage;

        public CreateChangeOfPartyRequestMapper(ICacheStorageService cacheStorage)
        {
            _cacheStorage = cacheStorage;
        }

        public async Task<CreateChangeOfPartyRequestRequest> Map(ConfirmViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            return new CreateChangeOfPartyRequestRequest
            {
                ChangeOfPartyRequestType = ChangeOfPartyRequestType.ChangeEmployer,
                NewPartyId = cacheItem.AccountLegalEntityId,
                NewPrice = cacheItem.Price,
                NewStartDate = new MonthYearModel(cacheItem.StartDate).Date.Value,
                NewEndDate = new MonthYearModel(cacheItem.EndDate).Date.Value,
                NewEmploymentEndDate = string.IsNullOrWhiteSpace(cacheItem.EmploymentEndDate) ? default : new MonthYearModel(cacheItem.EmploymentEndDate).Date.Value,
                NewEmploymentPrice = cacheItem.EmploymentPrice,
            };
        }
    }
}
