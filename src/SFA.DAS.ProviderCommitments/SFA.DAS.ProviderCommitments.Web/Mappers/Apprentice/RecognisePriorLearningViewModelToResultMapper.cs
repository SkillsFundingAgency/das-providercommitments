using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class RecognisePriorLearningViewModelToResultMapper : IMapper<RecognisePriorLearningViewModel, RecognisePriorLearningResult>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public RecognisePriorLearningViewModelToResultMapper(ICommitmentsApiClient commitmentsApiClient)
            => _commitmentsApiClient = commitmentsApiClient;

        public async Task<RecognisePriorLearningResult> Map(RecognisePriorLearningViewModel source)
        {
            var update = _commitmentsApiClient.RecognisePriorLearning(
                source.CohortId,
                source.DraftApprenticeshipId,
                new CommitmentsV2.Api.Types.Requests.RecognisePriorLearningRequest
                {
                    RecognisePriorLearning = source.IsTherePriorLearning
                });

            var apprenticeship = _commitmentsApiClient.GetDraftApprenticeship(
                source.CohortId,
                source.DraftApprenticeshipId,
                CancellationToken.None);

            await Task.WhenAll(update, apprenticeship);

            return new RecognisePriorLearningResult
            {
                HasStandardOptions = apprenticeship.Result.HasStandardOptions
            };
        }
    }

    public class PriorLearningDataViewModelToResultMapper : IMapper<PriorLearningDataViewModel, RecognisePriorLearningResult>
    {
        private readonly IOuterApiService _outerApiService;

        public PriorLearningDataViewModelToResultMapper(IOuterApiService outerApiService)
        {
            _outerApiService = outerApiService;
        }

        public async Task<RecognisePriorLearningResult> Map(PriorLearningDataViewModel source)
        {
            var result = new RecognisePriorLearningResult();

            var update = await _outerApiService.UpdatePriorLearningData(source.ProviderId, source.CohortId, source.DraftApprenticeshipId,
                new CreatePriorLearningDataApimRequest
                {
                    DurationReducedBy = source.DurationReducedBy,
                    CostBeforeRpl = source.CostBeforeRpl,
                    DurationReducedByHours = source.DurationReducedByHours,
                    IsDurationReducedByRpl = source.IsDurationReducedByRpl,
                    PriceReducedBy = source.PriceReduced,
                    TrainingTotalHours = source.TrainingTotalHours
                }
            );

            if (update != null)
            {
                result.HasStandardOptions = update.HasStandardOptions;
                result.RplPriceReductionError = update.RplPriceReductionError;
            }

            return result;
        }
    }
}