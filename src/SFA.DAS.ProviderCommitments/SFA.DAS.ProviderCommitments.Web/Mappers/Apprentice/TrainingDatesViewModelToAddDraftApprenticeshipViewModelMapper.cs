using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelToAddDraftApprenticeshipViewModelMapper : IMapper<TrainingDatesViewModel, AddDraftApprenticeshipViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public TrainingDatesViewModelToAddDraftApprenticeshipViewModelMapper(IOuterApiClient outerApiClient,
            ICommitmentsApiClient commitmentsApiClient,
            ICacheStorageService cacheStorage)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(TrainingDatesViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, cacheItem.AccountLegalEntityId, apprenticeship.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            var result = new AddDraftApprenticeshipViewModel
            {
                IsChangeOfEmployer = true,
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                Courses = null,
                StartDate = new MonthYearModel(cacheItem.StartDate),
                CourseCode = apprenticeship.CourseCode,
                DeliveryModel = (DeliveryModel)cacheItem.DeliveryModel.Value,
                HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                Employer = apiResponse.LegalEntityName,
                AccountLegalEntityId = cacheItem.AccountLegalEntityId,
                IsOnFlexiPaymentPilot = apprenticeship.IsOnFlexiPaymentPilot,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                Email = apprenticeship.Email,
                Uln = apprenticeship.Uln,
                TrainingPrice = null,
                EndPointAssessmentPrice = null,
                EmploymentPrice = cacheItem.EmploymentPrice,
                BirthDay = apprenticeship.DateOfBirth.Day,
                BirthMonth = apprenticeship.DateOfBirth.Month,
                BirthYear = apprenticeship.DateOfBirth.Year              
            };

            if (source.StartDate.HasValue)
            {
                result.StartMonth = source.StartDate.Month;
                result.StartYear = source.StartDate.Year;
            }

            if (source.EndDate.HasValue)
            {
                result.EndDay = source.EndDate.Day;
                result.EndMonth = source.EndDate.Month;
                result.EndYear = source.EndDate.Year;
            }
          
            if (source.EmploymentEndDate.HasValue)
            {
                result.EmploymentEndMonth = source.EmploymentEndDate.Month;
                result.EmploymentEndYear = source.EmploymentEndDate.Year;
            }

            return result;
        }
    }
}
