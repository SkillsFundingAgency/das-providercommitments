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
     IConfiguration configuration,
     ILogger<CreateCohortRedirectModelMapper> logger)
     : IMapper<CreateCohortWithDraftApprenticeshipRequest, CreateCohortRedirectModel>
 {
     public async Task<CreateCohortRedirectModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            CreateCohortRedirectModel.RedirectTarget RedirectTo(bool isOnFlexiPaymentPiolt)
            {
                if (isOnFlexiPaymentPiolt)
                {
                    return CreateCohortRedirectModel.RedirectTarget.ChooseFlexiPaymentPilotStatus;
                }

                if (configuration.GetValue<bool>("ILRFeaturesEnabled") == false)
                {
                    return CreateCohortRedirectModel.RedirectTarget.SelectCourse;
                }

                if(source.UseLearnerData.HasValue == false)
                {
                    return CreateCohortRedirectModel.RedirectTarget.SelectHowTo;
                }

                return source.UseLearnerData == true
                    ? CreateCohortRedirectModel.RedirectTarget.SelectLearner
                    : CreateCohortRedirectModel.RedirectTarget.SelectCourse;
            }

            var flexiPaymentsAuthorized = await authorizationService.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot);
            logger.LogInformation("Returning CreateCohortRedirectModel, isOnFlexiPaymentPilot {0}, UseLearnerData {1}", flexiPaymentsAuthorized, source.UseLearnerData);

            var cacheKey = Guid.NewGuid();
            var cacheItem = new CreateCohortCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId.Value,
                StartMonthYear = source.StartMonthYear,
                AccountLegalEntityId = source.AccountLegalEntityId,
                UseLearnerData = source.UseLearnerData,
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
