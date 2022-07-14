using System;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmViewModelMapper : IMapper<ConfirmRequest, ConfirmViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly ICacheStorageService _cacheStorage;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<ConfirmViewModelMapper> _logger;

        public ConfirmViewModelMapper(ICommitmentsApiClient commitmentsApiClient, ILogger<ConfirmViewModelMapper> logger, ICacheStorageService cacheStorage, IEncodingService encodingService)
        {
            _commitmentApiClient = commitmentsApiClient;
            _logger = logger;
            _cacheStorage = cacheStorage;
            _encodingService = encodingService;
        }

        public async Task<ConfirmViewModel> Map(ConfirmRequest source)
        {
            try
            {
                var cacheItem = await _cacheStorage.RetrieveFromCache<ChangeEmployerCacheItem>(source.CacheKey);

                var data = await  GetApprenticeshipData(source.ApprenticeshipId, cacheItem);

                var newStartDate = new MonthYearModel(cacheItem.StartDate);
                var newEndDate = new MonthYearModel(cacheItem.EndDate);
                var newEmploymentEndDate = string.IsNullOrEmpty(cacheItem.EmploymentEndDate)
                    ? null
                    : new MonthYearModel(cacheItem.EmploymentEndDate);

                return new ConfirmViewModel
                {
                    DeliveryModel = cacheItem.DeliveryModel.Value,
                    OldDeliveryModel = data.Apprenticeship.DeliveryModel,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    AccountLegalEntityPublicHashedId = _encodingService.Encode(cacheItem.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                    OldEmployerName = data.Apprenticeship.EmployerName,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    StopDate = data.Apprenticeship.StopDate.Value, 
                    OldStartDate = data.Apprenticeship.StartDate,
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
                    FundingBandCap = GetFundingBandCap(data.TrainingProgramme, newStartDate.Date),
                    ShowDeliveryModel = !cacheItem.SkippedDeliveryModelSelection ||
                                        (cacheItem.SkippedDeliveryModelSelection && cacheItem.DeliveryModel == DeliveryModel.Regular),
                    ShowDeliveryModelChangeLink = !cacheItem.SkippedDeliveryModelSelection,
                    CacheKey = source.CacheKey
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Error mapping apprenticeshipId {source.ApprenticeshipId} to model {nameof(ConfirmViewModel)}", e);
                throw;
            }
        }

        private async Task<(GetApprenticeshipResponse Apprenticeship,
           GetPriceEpisodesResponse PriceEpisodes,
           AccountLegalEntityResponse AccountLegalEntity,
           TrainingProgramme TrainingProgramme)>
           GetApprenticeshipData(long apprenticeshipId, ChangeEmployerCacheItem cacheItem)
        {
            var apprenticeship = await _commitmentApiClient.GetApprenticeship(apprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(apprenticeshipId);
            var legalEntityTask =  _commitmentApiClient.GetAccountLegalEntity(cacheItem.AccountLegalEntityId);
            var trainingProgrammeTask = _commitmentApiClient.GetTrainingProgramme(apprenticeship.CourseCode);

            await Task.WhenAll(priceEpisodesTask, legalEntityTask, trainingProgrammeTask);

            var priceEpisodes = priceEpisodesTask.Result;
            var legalEntity = legalEntityTask.Result;
            var course = trainingProgrammeTask.Result;

            return (apprenticeship,
                priceEpisodes,
                legalEntity,
                course.TrainingProgramme);
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