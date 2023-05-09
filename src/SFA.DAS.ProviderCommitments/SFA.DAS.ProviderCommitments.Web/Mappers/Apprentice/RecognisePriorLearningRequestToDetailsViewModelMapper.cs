using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
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

            return new PriorLearningDataViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                TrainingTotalHours = priorLearningData.TrainingTotalHours,
                DurationReducedByHours = priorLearningData.DurationReducedByHours,
                IsDurationReducedByRpl = priorLearningData.IsDurationReducedByRpl,
                DurationReducedBy = priorLearningData.ReducedDuration,
                CostBeforeRpl = priorLearningData.CostBeforeRpl,
                PriceReduced = priorLearningData.PriceReduced,
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
            var priorLearningSummary = await _outerApiService.GetPriorLearningSummary(source.ProviderId, source.CohortId, source.DraftApprenticeshipId);

            return new PriorLearningSummaryViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                TrainingTotalHours = priorLearningSummary.TrainingTotalHours,
                DurationReducedByHours = priorLearningSummary.DurationReducedByHours,
                CostBeforeRpl = priorLearningSummary.CostBeforeRpl,
                PriceReducedBy = priorLearningSummary.PriceReducedBy,
                FundingBandMaximum = priorLearningSummary.FundingBandMaximum,
                PercentageOfPriorLearning = priorLearningSummary.PercentageOfPriorLearning,
                MinimumPercentageReduction = priorLearningSummary.MinimumPercentageReduction,
                MinimumPriceReduction = priorLearningSummary.MinimumPriceReduction,
                RplPriceReductionError = priorLearningSummary.RplPriceReductionError,
                TotalCost = priorLearningSummary.TotalCost,
                FullName = string.Format("{0} {1}", priorLearningSummary.FirstName, priorLearningSummary.LastName),
                HasStandardOptions = priorLearningSummary.HasStandardOptions
            };
        }
    }
}