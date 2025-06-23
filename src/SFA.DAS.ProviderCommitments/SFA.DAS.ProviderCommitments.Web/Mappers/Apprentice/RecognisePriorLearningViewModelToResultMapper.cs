using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class RecognisePriorLearningViewModelToResultMapper(ICommitmentsApiClient commitmentsApiClient)
    : IMapper<RecognisePriorLearningViewModel, RecognisePriorLearningResult>
{
    public async Task<RecognisePriorLearningResult> Map(RecognisePriorLearningViewModel source)
    {
        await commitmentsApiClient.RecognisePriorLearning(
            source.CohortId,
            source.DraftApprenticeshipId,
            new CommitmentsV2.Api.Types.Requests.RecognisePriorLearningRequest
            {
                RecognisePriorLearning = source.IsTherePriorLearning
            });


        return new RecognisePriorLearningResult
        {
        };
    }
}

public class PriorLearningDataViewModelToResultMapper(IOuterApiService outerApiService)
    : IMapper<PriorLearningDataViewModel, RecognisePriorLearningResult>
{
    public async Task<RecognisePriorLearningResult> Map(PriorLearningDataViewModel source)
    {
        var result = new RecognisePriorLearningResult();

        var update = await outerApiService.UpdatePriorLearningData(source.ProviderId, source.CohortId, source.DraftApprenticeshipId,
            new CreatePriorLearningDataRequest
            {
                DurationReducedBy = source.IsDurationReducedByRpl == false ? null : source.DurationReducedBy,
                CostBeforeRpl = source.CostBeforeRpl,
                DurationReducedByHours = source.DurationReducedByHours,
                IsDurationReducedByRpl = source.IsDurationReducedByRpl,
                PriceReducedBy = source.PriceReduced,
                TrainingTotalHours = source.TrainingTotalHours
            }
        );

        if (update != null)
        {
            result.RplPriceReductionError = update.RplPriceReductionError;
        }

        return result;
    }
}