using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipRedirectModelMapper : IMapper<AddDraftApprenticeshipOrRoutePostRequest, AddDraftApprenticeshipRedirectModel>
    {
        private readonly ICacheStorageService _cacheStorage;
        private readonly IOuterApiService _apiService;

        public AddDraftApprenticeshipRedirectModelMapper(ICacheStorageService cacheStorage, IOuterApiService apiService)
        {
            _cacheStorage = cacheStorage;
            _apiService = apiService;
        }

        public async Task<AddDraftApprenticeshipRedirectModel> Map(AddDraftApprenticeshipOrRoutePostRequest source)
        {
            await AddToCache(source);

            if (source.IsChangeOptionSelected)
            {
                return CreateRedirectModelForEdit(source);
            }

            var overlapResult = await HasStartDateOverlap(source);
            if (overlapResult != null && overlapResult.HasStartDateOverlap && overlapResult.HasOverlapWithApprenticeshipId.HasValue)
            {
                return new AddDraftApprenticeshipRedirectModel
                {
                    CacheKey = source.CacheKey,
                    ProviderId = source.ProviderId,
                    RedirectTo = AddDraftApprenticeshipRedirectModel.RedirectTarget.OverlapWarning,
                    OverlappingApprenticeshipId = overlapResult.HasOverlapWithApprenticeshipId.Value
                };
            }

            return new AddDraftApprenticeshipRedirectModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                IsEdit = true,
                RedirectTo = AddDraftApprenticeshipRedirectModel.RedirectTarget.SaveApprenticeship
            };
        }

        private static AddDraftApprenticeshipRedirectModel CreateRedirectModelForEdit(AddDraftApprenticeshipOrRoutePostRequest source)
        {
            var result = new AddDraftApprenticeshipRedirectModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                RedirectTo = source.IsChangeCourse
                    ? AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectCourse
                    : AddDraftApprenticeshipRedirectModel.RedirectTarget.SelectDeliveryModel
            };

            return result;
        }

        private async Task AddToCache(AddDraftApprenticeshipOrRoutePostRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);
            cacheItem.FirstName = source.FirstName;
            cacheItem.LastName = source.LastName;
            cacheItem.StartMonthYear = source.StartDate.MonthYear;
            cacheItem.Email = source.Email;
            cacheItem.Uln = source.Uln;
            cacheItem.StartDate = source.StartDate.Date;
            cacheItem.EndDate = source.EndDate.Date;
            cacheItem.DateOfBirth = source.DateOfBirth.Date;
            cacheItem.ActualStartDate = source.ActualStartDate.Date;
            cacheItem.EmploymentEndDate = source.EmploymentEndDate.Date;
            cacheItem.EmploymentPrice = source.EmploymentPrice;
            cacheItem.Cost = source.Cost;
            cacheItem.Reference = source.Reference;
            cacheItem.TrainingPrice = source.TrainingPrice;
            cacheItem.EndPointAssessmentPrice = source.EndPointAssessmentPrice;

            await _cacheStorage.SaveToCache(cacheItem.CacheKey, cacheItem, 1);
        }

        private async Task<Infrastructure.OuterApi.Responses.ValidateUlnOverlapOnStartDateQueryResult> HasStartDateOverlap(AddDraftApprenticeshipViewModel model)
        {
            if (!model.StartDate.Date.HasValue || !model.EndDate.Date.HasValue || string.IsNullOrWhiteSpace(model.Uln))
            {
                return null;
            }
            
            //this request should be moved to a GET request on the outer api following the BFF pattern
            //start date, end date and ULN would be qs parameters that can be used to determine if there is an overlap
            //and if so, provide the ID of the overlapping apprenticeship

            var apimRequest = model.MapDraftApprenticeship();
            await _apiService.ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(apimRequest);

            var result = await _apiService.ValidateUlnOverlapOnStartDate(
                model.ProviderId,
                model.Uln,
                model.StartDate.Date.Value.ToString("dd-MM-yyyy"),
                model.EndDate.Date.Value.ToString("dd-MM-yyyy")
            );

            return result;

        }
    }
}