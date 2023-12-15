using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SentRequestMapper : IMapper<ConfirmViewModel, SentRequest>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly IAuthenticationService _authenticationService;

        public SentRequestMapper(IOuterApiClient outerApiClient, ICacheStorageService cacheStorage, IAuthenticationService authenticationService)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
            _authenticationService = authenticationService;
        }

        public async Task<SentRequest> Map(ConfirmViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            var apiRequestBody = new PostCreateChangeOfEmployerRequest.Body
            {
                AccountLegalEntityId = cacheItem.AccountLegalEntityId,
                Price = cacheItem.Price,
                StartDate = new MonthYearModel(cacheItem.StartDate).Date.Value,
                EndDate = new MonthYearModel(cacheItem.EndDate).Date.Value,
                EmploymentEndDate = string.IsNullOrWhiteSpace(cacheItem.EmploymentEndDate) ? default : new MonthYearModel(cacheItem.EmploymentEndDate).Date.Value,
                EmploymentPrice = cacheItem.EmploymentPrice,
                DeliveryModel = cacheItem.DeliveryModel,
                UserInfo = new ApimUserInfo
                {
                    UserDisplayName = _authenticationService.UserName,
                    UserEmail = _authenticationService.UserEmail,
                    UserId = _authenticationService.UserId
                }
            };

            var apiRequest = new PostCreateChangeOfEmployerRequest(source.ProviderId, source.ApprenticeshipId, apiRequestBody);

            await _outerApiClient.Post<PostCreateChangeOfEmployerResponse>(apiRequest);

            await _cacheStorage.DeleteFromCache(source.CacheKey.ToString());

            return new SentRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId
            };
        }
    }
}
