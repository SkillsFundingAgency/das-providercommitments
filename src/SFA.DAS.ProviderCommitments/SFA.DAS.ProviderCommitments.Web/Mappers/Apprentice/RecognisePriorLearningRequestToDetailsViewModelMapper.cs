using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using StackExchange.Redis;
using System.Security.Policy;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class RecognisePriorLearningRequestToDataViewModelMapper : IMapper<RecognisePriorLearningRequest, PriorLearningDataViewModel>
    {
        private readonly IOuterApiService _outerApiService;

        public RecognisePriorLearningRequestToDataViewModelMapper(IOuterApiService outerApiService)
        {
            _outerApiService = outerApiService;
        }

        public async Task<PriorLearningDataViewModel> Map(RecognisePriorLearningRequest source)
        {
            var priorLearningData = await _outerApiService.GetPriorLearningData(source.ProviderId, source.CohortId, source.DraftApprenticeshipId);

            var result = new PriorLearningDataViewModel();
            result.CohortId = source.CohortId;
            result.CohortReference = source.CohortReference;
            result.DraftApprenticeshipId = source.DraftApprenticeshipId;
            result.ProviderId = source.ProviderId;
            result.DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId;

            if (priorLearningData != null)
            {
                result.TrainingTotalHours = priorLearningData.TrainingTotalHours;
                result.DurationReducedByHours = priorLearningData.DurationReducedByHours;
                result.IsDurationReducedByRpl = priorLearningData.IsDurationReducedByRpl;
                result.DurationReducedBy = priorLearningData.DurationReducedBy;
                result.CostBeforeRpl = priorLearningData.CostBeforeRpl;
                result.PriceReduced = priorLearningData.PriceReduced;
            }

            if (result?.DurationReducedBy == 0 && result.PriceReduced.HasValue)
            {
                result.IsDurationReducedByRpl = false;
                result.DurationReducedBy = null;
            }

            return result;
        }
    }

    public class RecognisePriorLearningRequestToDetailsViewModelMapper : IMapper<RecognisePriorLearningRequest, PriorLearningDetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        public RecognisePriorLearningRequestToDetailsViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }
        public async Task<PriorLearningDetailsViewModel> Map(RecognisePriorLearningRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            return new PriorLearningDetailsViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                WeightageReducedBy = apprenticeship.WeightageReducedBy,
                QualificationsForRplReduction = apprenticeship.QualificationsForRplReduction,
                ReasonForRplReduction = apprenticeship.ReasonForRplReduction,
                DurationReducedByHours = apprenticeship.DurationReducedByHours,
                ReducedDuration = apprenticeship.DurationReducedBy,
                ReducedPrice = apprenticeship.PriceReducedBy,
            };
        }
    }

    public class RecognisePriorLearningSummaryRequestToSummaryViewModelMapper : IMapper<PriorLearningSummaryRequest, PriorLearningSummaryViewModel>
    {
        private readonly IOuterApiService _outerApiService;

        public RecognisePriorLearningSummaryRequestToSummaryViewModelMapper(IOuterApiService outerApiService)
        {
            _outerApiService = outerApiService;
        }

        public async Task<PriorLearningSummaryViewModel> Map(PriorLearningSummaryRequest source)
        {
            var result = new PriorLearningSummaryViewModel();
            var priorLearningSummary = await _outerApiService.GetPriorLearningSummary(source.ProviderId, source.CohortId, source.DraftApprenticeshipId);

            result.CohortId = source.CohortId;
            result.CohortReference = source.CohortReference;
            result.DraftApprenticeshipId = source.DraftApprenticeshipId;
            result.ProviderId = source.ProviderId;
            result.DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId;

            if (priorLearningSummary != null)
            {
                result.TrainingTotalHours = priorLearningSummary.TrainingTotalHours;
                result.DurationReducedByHours = priorLearningSummary.DurationReducedByHours;
                result.CostBeforeRpl = priorLearningSummary.CostBeforeRpl;
                result.PriceReducedBy = priorLearningSummary.PriceReducedBy;
                result.FundingBandMaximum = priorLearningSummary.FundingBandMaximum;
                result.PercentageOfPriorLearning = priorLearningSummary.PercentageOfPriorLearning;
                result.MinimumPercentageReduction = priorLearningSummary.MinimumPercentageReduction;
                result.MinimumPriceReduction = priorLearningSummary.MinimumPriceReduction;
                result.RplPriceReductionError = priorLearningSummary.RplPriceReductionError;
                result.TotalCost = priorLearningSummary.TotalCost;
                result.FullName = $"{priorLearningSummary.FirstName} {priorLearningSummary.LastName}";
                result.HasStandardOptions = priorLearningSummary.HasStandardOptions;
            }
 
            return result;
        }
    }
}