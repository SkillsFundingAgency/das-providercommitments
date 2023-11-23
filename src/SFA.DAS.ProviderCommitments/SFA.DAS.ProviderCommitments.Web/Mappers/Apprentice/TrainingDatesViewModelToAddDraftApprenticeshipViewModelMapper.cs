using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class TrainingDatesViewModelToAddDraftApprenticeshipViewModelMapper : IMapper<TrainingDatesViewModel, AddDraftApprenticeshipViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;

        public TrainingDatesViewModelToAddDraftApprenticeshipViewModelMapper(IOuterApiClient outerApiClient, ICommitmentsApiClient commitmentsApiClient, ICacheStorageService cacheStorage, IEncodingService encodingService)
        {
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(TrainingDatesViewModel source)
        {
            var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);


            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, cacheItem.AccountLegalEntityId, apprenticeship.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            var result = new AddDraftApprenticeshipViewModel
            {
                CacheKey = source.CacheKey,
                ProviderId = source.ProviderId,
                //CohortId = apprenticeship.CohortId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                Courses = null,
               // ReservationId = cacheItem.ReservationId,
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
                //Reference = _encodingService.Encode(apprenticeship.CohortId, EncodingType.CohortReference),
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
