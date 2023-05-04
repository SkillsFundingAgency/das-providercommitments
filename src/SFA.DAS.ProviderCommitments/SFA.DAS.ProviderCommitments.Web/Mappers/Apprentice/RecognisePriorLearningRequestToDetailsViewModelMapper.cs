using Azure.Core;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
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
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public RecognisePriorLearningRequestToDataViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<PriorLearningDataViewModel> Map(RecognisePriorLearningRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            var reducedDuration = apprenticeship.DurationReducedBy;
            var isDurationReducedByRpl = apprenticeship.IsDurationReducedByRpl;

            if (isDurationReducedByRpl == null && reducedDuration != null)
            {
                isDurationReducedByRpl = true;
            }

            if (isDurationReducedByRpl == false && reducedDuration != null)
            {
                reducedDuration = null;
            }

            return new PriorLearningDataViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                TrainingTotalHours = apprenticeship.TrainingTotalHours,
                DurationReducedByHours = apprenticeship.DurationReducedByHours,
                IsDurationReducedByRpl = isDurationReducedByRpl,
                DurationReducedBy = reducedDuration,
                CostBeforeRpl = apprenticeship.CostBeforeRpl,
                PriceReduced = apprenticeship.PriceReducedBy,
            };
        }
    }

    public class RecognisePriorLearningSummaryRequestToSummaryViewModelMapper : IMapper<PriorLearningSummaryRequest, PriorLearningSummaryViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiService _outerApiService;

        public RecognisePriorLearningSummaryRequestToSummaryViewModelMapper(IOuterApiService outerApiService, ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _outerApiService = outerApiService;
        }

        public async Task<PriorLearningSummaryViewModel> Map(PriorLearningSummaryRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            var priorLearningSummary = await _outerApiService.GetPriorLearningSummary(source.CohortId, source.DraftApprenticeshipId);

            //#if DEBUG
            //if (priorLearningSummary == null)
            //{
            //    priorLearningSummary = new Infrastructure.OuterApi.Responses.GetPriorLearningSummaryQueryResult();
            //    priorLearningSummary.PercentageOfPriorLearning = 15;
            //    priorLearningSummary.TrainingTotalHours = 8;
            //    priorLearningSummary.DurationReducedByHours = 5;
            //    priorLearningSummary.CostBeforeRpl = 1000;
            //    priorLearningSummary.PriceReducedBy = 10;
            //    priorLearningSummary.FundingBandMaximum = 25;
            //    priorLearningSummary.PercentageOfPriorLearning = 18;
            //    priorLearningSummary.MinimumPercentageReduction = 65;
            //    priorLearningSummary.MinimumPriceReduction = 55;
            //    priorLearningSummary.RplPriceReductionError = true;
            //}
            //#endif

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
                TotalCost = apprenticeship.Cost,
                FullName = string.Format("{0} {1}", apprenticeship.FirstName, apprenticeship.LastName),
                HasStandardOptions = apprenticeship.HasStandardOptions
            };
        }
    }
}