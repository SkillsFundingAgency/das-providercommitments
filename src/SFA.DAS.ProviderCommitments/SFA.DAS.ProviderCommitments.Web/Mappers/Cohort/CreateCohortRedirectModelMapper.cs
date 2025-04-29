using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
 public class CreateCohortRedirectModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, CreateCohortRedirectModel>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ICacheStorageService _cacheStorageService;

        public CreateCohortRedirectModelMapper(IAuthorizationService authorizationService, ICacheStorageService cacheStorageService)
        {
            _authorizationService = authorizationService;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<CreateCohortRedirectModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            CreateCohortRedirectModel.RedirectTarget RedirectTo(bool isOnFlexiPaymentPiolt)
            {
                if (isOnFlexiPaymentPiolt)
                {
                    return CreateCohortRedirectModel.RedirectTarget.ChooseFlexiPaymentPilotStatus;
                }

                return source.UseIlrData == true
                    ? CreateCohortRedirectModel.RedirectTarget.SelectLearner
                    : CreateCohortRedirectModel.RedirectTarget.SelectCourse;
            }

            var flexiPaymentsAuthorized = await _authorizationService.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot);

            var cacheKey = Guid.NewGuid();
            var cacheItem = new CreateCohortCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartMonthYear,
                AccountLegalEntityId = source.AccountLegalEntityId,
                UseIlrData = source.UseIlrData,
                IsOnFlexiPaymentPilot = flexiPaymentsAuthorized ? null : false
            };
            await _cacheStorageService.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortRedirectModel
            {
                CacheKey = cacheKey,
                RedirectTo = RedirectTo(flexiPaymentsAuthorized)
            };
        }
    }
}
