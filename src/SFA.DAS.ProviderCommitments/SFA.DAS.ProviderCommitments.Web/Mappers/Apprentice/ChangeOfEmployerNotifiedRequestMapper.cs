using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangeOfEmployerNotifiedRequestMapper : IMapper<OverlapOptionsForChangeEmployerViewModel, ChangeOfEmployerNotifiedRequest>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly IAuthenticationService _authenticationService;

        public ChangeOfEmployerNotifiedRequestMapper(IOuterApiClient outerApiClient, ICacheStorageService cacheStorage, IAuthenticationService authenticationService)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
            _authenticationService = authenticationService;
        }

        public async Task<ChangeOfEmployerNotifiedRequest> Map(OverlapOptionsForChangeEmployerViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

            var apiRequestBody = new PostConfirmRequest.Body
            {
                AccountLegalEntityId = cacheItem.AccountLegalEntityId,
                Price = cacheItem.Price,
                StartDate = new MonthYearModel(cacheItem.StartDate).Date.Value,
                EndDate = new MonthYearModel(cacheItem.EndDate).Date.Value,
                EmploymentEndDate = string.IsNullOrWhiteSpace(cacheItem.EmploymentEndDate) ? default : new MonthYearModel(cacheItem.EmploymentEndDate).Date.Value,
                EmploymentPrice = cacheItem.EmploymentPrice,
                DeliveryModel = cacheItem.DeliveryModel,
                HasOLTD = true,
                UserInfo = new ApimUserInfo
                {
                    UserDisplayName = _authenticationService.UserName,
                    UserEmail = _authenticationService.UserEmail,
                    UserId = _authenticationService.UserId
                }
            };

            var apiRequest = new PostConfirmRequest(source.ProviderId, source.ApprenticeshipId.Value,
                apiRequestBody);

            await _outerApiClient.Post<PostConfirmResponse>(apiRequest);

            await _cacheStorage.DeleteFromCache(source.CacheKey.ToString());

            return new ChangeOfEmployerNotifiedRequest
            {
                ProviderId = source.ProviderId
            };
        }
    }
}

