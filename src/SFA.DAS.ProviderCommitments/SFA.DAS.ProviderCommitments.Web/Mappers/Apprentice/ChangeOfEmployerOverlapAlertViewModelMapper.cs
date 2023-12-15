using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class
        ChangeOfEmployerOverlapAlertViewModelMapper : IMapper<ChangeOfEmployerOverlapAlertRequest,
            ChangeOfEmployerOverlapAlertViewModel>
    {
        private readonly ICacheStorageService _cacheStorage;
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _outerApiClient;
        private readonly ILogger<ChangeOfEmployerOverlapAlertViewModelMapper> _logger;

        public ChangeOfEmployerOverlapAlertViewModelMapper(IOuterApiClient outerApiClient,
            ILogger<ChangeOfEmployerOverlapAlertViewModelMapper> logger, ICacheStorageService cacheStorage,
            IEncodingService encodingService)
        {
            _logger = logger;
            _cacheStorage = cacheStorage;
            _encodingService = encodingService;
            _outerApiClient = outerApiClient;
        }

        public async Task<ChangeOfEmployerOverlapAlertViewModel> Map(ChangeOfEmployerOverlapAlertRequest source)
        {
            try
            {
                var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

                var data = await GetApprenticeshipData(source.ProviderId, source.ApprenticeshipId, cacheItem.AccountLegalEntityId);

                var newStartDate = new MonthYearModel(cacheItem.StartDate);
                var newEndDate = new MonthYearModel(cacheItem.EndDate);
                var newEmploymentEndDate = string.IsNullOrEmpty(cacheItem.EmploymentEndDate)
                    ? null
                    : new MonthYearModel(cacheItem.EmploymentEndDate);

                return new ChangeOfEmployerOverlapAlertViewModel
                {
                    DeliveryModel = cacheItem.DeliveryModel.Value,
                    OldDeliveryModel = data.Apprenticeship.DeliveryModel,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    AccountLegalEntityPublicHashedId = _encodingService.Encode(cacheItem.AccountLegalEntityId,
                        EncodingType.PublicAccountLegalEntityId),
                    OldEmployerName = data.Apprenticeship.EmployerName,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    StopDate = data.Apprenticeship.StopDate,
                    OldStartDate = data.Apprenticeship.StartDate.Value,
                    OldEndDate = data.Apprenticeship.EndDate,
                    OldPrice = decimal.ToInt32(data.PriceEpisodes.PriceEpisodes.GetPrice()),
                    OldEmploymentPrice = data.Apprenticeship.EmploymentPrice,
                    OldEmploymentEndDate = data.Apprenticeship.EmploymentEndDate,
                    NewEmployerName = data.AccountLegalEntity.LegalEntityName,
                    NewStartDate = newStartDate.MonthYear,
                    NewEndDate = newEndDate.MonthYear,
                    NewPrice = cacheItem.Price.Value,
                    NewEmploymentEndDate = newEmploymentEndDate?.MonthYear,
                    NewEmploymentPrice = cacheItem.EmploymentPrice,
                    FundingBandCap = GetFundingBandCap(data.TrainingProgrammeResponse.TrainingProgramme, newStartDate.Date),
                    ShowDeliveryModel = !cacheItem.SkippedDeliveryModelSelection ||
                                        (cacheItem.SkippedDeliveryModelSelection && (int)cacheItem.DeliveryModel !=
                                            (int)data.Apprenticeship.DeliveryModel),
                    ShowDeliveryModelChangeLink = !cacheItem.SkippedDeliveryModelSelection,
                    CacheKey = source.CacheKey
                };
            }
            catch (Exception e)
            {
                _logger.LogError(
                    $"Error mapping apprenticeshipId {source.ApprenticeshipId} to model {nameof(ChangeOfEmployerOverlapAlertViewModel)}",
                    e);
                throw;
            }
        }

        private async Task<GetApprenticeshipDataResponse> GetApprenticeshipData(long providerId, long apprenticeshipId, long accountLegalEntityId)
        {
            var apprenticeshipDetails = await _outerApiClient.Get<GetApprenticeshipDataResponse>(
                new GetApprenticeshipDataRequest(providerId, apprenticeshipId, accountLegalEntityId));

            return apprenticeshipDetails;
        }

        private int? GetFundingBandCap(TrainingProgramme course, DateTime? startDate)
        {
            if (course == null)
            {
                return null;
            }

            var cap = course.FundingCapOn(startDate.Value);

            if (cap > 0)
            {
                return cap;
            }

            return null;
        }
    }
}