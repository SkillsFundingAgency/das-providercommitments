using System.Linq;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.Cohort.SelectCourseViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectCourseViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectCourseViewModel>
    {
        private readonly IOuterApiClient _apiClient;
        private readonly ICacheStorageService _cacheStorage;

        public SelectCourseViewModelMapper(
            IOuterApiClient apiClient,
            ICacheStorageService cacheStorage)
        {
            _apiClient = apiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<SelectCourseViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheModel>(source.CacheKey);

            var apiRequest = new GetAddDraftApprenticeshipCourseRequest(source.ProviderId, source.AccountLegalEntityId);
            var apiResponse = await _apiClient.Get<GetAddDraftApprenticeshipCourseResponse>(apiRequest);

            var result = new SelectCourseViewModel
            {
                CacheKey = source.CacheKey,
                CourseCode = cacheItem.CourseCode,
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                IsOnFlexiPaymentsPilot = source.IsOnFlexiPaymentPilot,
                ShowManagingStandardsContent = apiResponse.IsMainProvider,
                Standards = apiResponse.Standards.Select(x => new Standard { CourseCode = x.CourseCode, Name = x.Name })
            };

            return result;
        }
    }
}
