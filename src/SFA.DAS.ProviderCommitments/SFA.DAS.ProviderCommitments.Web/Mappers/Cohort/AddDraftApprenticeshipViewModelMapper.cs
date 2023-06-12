using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public AddDraftApprenticeshipViewModelMapper(IOuterApiClient outerApiClient, ICacheStorageService cacheStorage)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<CreateCohortCacheItem>(source.CacheKey);

            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, cacheItem.AccountLegalEntityId, cacheItem.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            var result = new AddDraftApprenticeshipViewModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                Courses = null,
                ReservationId = cacheItem.ReservationId,
                StartDate = new MonthYearModel(cacheItem.StartMonthYear),
                CourseCode = cacheItem.CourseCode,
                DeliveryModel = (DeliveryModel) cacheItem.DeliveryModel.Value,
                HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                Employer = apiResponse.LegalEntityName,
                AccountLegalEntityId = cacheItem.AccountLegalEntityId,
                IsOnFlexiPaymentPilot = cacheItem.IsOnFlexiPaymentPilot,
                FirstName = cacheItem.FirstName,
                LastName = cacheItem.LastName,
                Email = cacheItem.Email,
                Uln = cacheItem.Uln,
                Cost = cacheItem.Cost,
                Reference = cacheItem.Reference,
                EmploymentPrice = cacheItem.EmploymentPrice
            };

            if (cacheItem.StartDate.HasValue)
            {
                result.StartMonth = cacheItem.StartDate.Value.Month;
                result.StartYear = cacheItem.StartDate.Value.Year;
            }

            if (cacheItem.EndDate.HasValue)
            {
                result.EndDay = cacheItem.EndDate.Value.Day;
                result.EndMonth = cacheItem.EndDate.Value.Month;
                result.EndYear = cacheItem.EndDate.Value.Year;
            }

            if (cacheItem.DateOfBirth.HasValue)
            {
                result.BirthDay = cacheItem.DateOfBirth.Value.Day;
                result.BirthMonth = cacheItem.DateOfBirth.Value.Month;
                result.BirthYear = cacheItem.DateOfBirth.Value.Year;
            }

            if (cacheItem.ActualStartDate.HasValue)
            {
                result.ActualStartDay = cacheItem.ActualStartDate.Value.Day;
                result.ActualStartMonth = cacheItem.ActualStartDate.Value.Month;
                result.ActualStartYear = cacheItem.ActualStartDate.Value.Year;
            }

            if (cacheItem.EmploymentEndDate.HasValue)
            {
                result.EmploymentEndMonth = cacheItem.EmploymentEndDate.Value.Month;
                result.EmploymentEndYear = cacheItem.EmploymentEndDate.Value.Year;
            }

            return result;
        }
    }
}