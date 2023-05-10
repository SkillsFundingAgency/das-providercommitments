using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
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