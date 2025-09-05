using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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
            CreateCohortRedirectModel.RedirectTarget RedirectTo()
            {
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

            logger.LogInformation("Returning CreateCohortRedirectModel, UseLearnerData {1}", source.UseLearnerData);

            var cacheKey = Guid.NewGuid();
            var cacheItem = new CreateCohortCacheItem(cacheKey)
            {
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartMonthYear,
                AccountLegalEntityId = source.AccountLegalEntityId,
                UseLearnerData = source.UseLearnerData,
                CourseCode = source.CourseCode
            };
            await cacheStorageService.SaveToCache(cacheItem.CacheKey, cacheItem, 1);

            return new CreateCohortRedirectModel
            {
                CacheKey = cacheKey,
                RedirectTo = RedirectTo()
            };
        }
    }
}
