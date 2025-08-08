using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
 public class CreateCohortRedirectModelMapper(
     IAuthorizationService authorizationService,
     ICacheStorageService cacheStorageService,
     ILogger<CreateCohortRedirectModelMapper> logger)
     : IMapper<CreateCohortWithDraftApprenticeshipRequest, CreateCohortRedirectModel>
 {
     public async Task<CreateCohortRedirectModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            CreateCohortRedirectModel.RedirectTarget RedirectTo(bool isOnFlexiPaymentPiolt)
            {
                return isOnFlexiPaymentPiolt ? CreateCohortRedirectModel.RedirectTarget.ChooseFlexiPaymentPilotStatus : CreateCohortRedirectModel.RedirectTarget.SelectLearner;
            }

            var flexiPaymentsAuthorized = await authorizationService.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot);

            var cacheKey = Guid.NewGuid();
            var cacheItem = new CreateCohortCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartMonthYear,
                AccountLegalEntityId = source.AccountLegalEntityId,
                CourseCode = source.CourseCode,
                IsOnFlexiPaymentPilot = flexiPaymentsAuthorized ? null : false
            };
            await cacheStorageService.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortRedirectModel
            {
                CacheKey = cacheKey,
                RedirectTo = RedirectTo(flexiPaymentsAuthorized)
            };
        }
    }
}
