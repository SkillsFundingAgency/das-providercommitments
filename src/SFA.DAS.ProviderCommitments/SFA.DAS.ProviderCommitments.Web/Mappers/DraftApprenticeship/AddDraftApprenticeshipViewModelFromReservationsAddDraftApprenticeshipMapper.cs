using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper : IMapper<ReservationsAddDraftApprenticeshipRequest, AddDraftApprenticeshipViewModel>
    {
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICacheStorageService _cacheStorage;

        public AddDraftApprenticeshipViewModelFromReservationsAddDraftApprenticeshipMapper(IEncodingService encodingService, IOuterApiClient outerApiClient, ICacheStorageService cacheStorage)
        {
            _encodingService = encodingService;
            _outerApiClient = outerApiClient;
            _cacheStorage = cacheStorage;
        }

        public async Task<AddDraftApprenticeshipViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
        {
            AddAnotherApprenticeshipCacheItem cacheItem = null;

            if (source.CacheKey.HasValue)
            {
                cacheItem = await _cacheStorage.RetrieveFromCache<AddAnotherApprenticeshipCacheItem>(source.CacheKey.ToString());
            }

            var apiRequest = new GetAddDraftApprenticeshipDetailsRequest(source.ProviderId, source.CohortId.Value, source.CourseCode);
            var apiResponse = await _outerApiClient.Get<GetAddDraftApprenticeshipDetailsResponse>(apiRequest);

            var model = new AddDraftApprenticeshipViewModel
            {
                AccountLegalEntityId = apiResponse.AccountLegalEntityId,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(apiResponse.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                Employer = apiResponse.LegalEntityName,
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                CohortId = source.CohortId,
                StartDate = new MonthYearModel(source.StartMonthYear),
                ReservationId = source.ReservationId,
                CourseCode = source.CourseCode,
                DeliveryModel = source.DeliveryModel,
                HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            };

            if (cacheItem != null)
            {
                model.IsOnFlexiPaymentPilot = false;
                model.ReservationId = cacheItem.ReservationId;
                model.FirstName = cacheItem.FirstName;
                model.LastName = cacheItem.LastName;
                model.Email = cacheItem.Email;

                if (cacheItem.DateOfBirth.HasValue)
                {
                    model.BirthDay = cacheItem.DateOfBirth.Value.Day;
                    model.BirthMonth = cacheItem.DateOfBirth.Value.Month;
                    model.BirthYear = cacheItem.DateOfBirth.Value.Year;
                }

                model.Uln = cacheItem.Uln;
                model.CourseCode = cacheItem.CourseCode;
                model.Cost = cacheItem.Cost;
                model.TrainingPrice = cacheItem.TrainingPrice;
                model.EndPointAssessmentPrice = cacheItem.EndPointAssessmentPrice;

                if (cacheItem.StartDate.HasValue)
                {
                    model.StartMonth = cacheItem.StartDate.Value.Month;
                    model.StartYear = cacheItem.StartDate.Value.Year;
                }

                if (cacheItem.EndDate.HasValue)
                {
                    model.EndMonth = cacheItem.EndDate.Value.Month;
                    model.EndYear = cacheItem.EndDate.Value.Year;
                }

                model.DeliveryModel = cacheItem.DeliveryModel == DeliveryModel.FlexiJobAgency
                    ? CommitmentsV2.Types.DeliveryModel.FlexiJobAgency
                    : CommitmentsV2.Types.DeliveryModel.Regular;
            }

            return model;

        }
    }
}