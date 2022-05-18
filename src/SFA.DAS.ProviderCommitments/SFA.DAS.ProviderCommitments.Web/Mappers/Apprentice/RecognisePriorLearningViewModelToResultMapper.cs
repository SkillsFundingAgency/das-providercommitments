using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
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
            await _commitmentsApiClient.UpdateDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId, null, CancellationToken.None);
            var apprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId, CancellationToken.None);

            //var nextPage = await _commitmentsApiClient.RecognisePriorLearning(
            //    request.CohortId,
            //    request.DraftApprenticeshipId,
            //    request.IsTherePriorLearning);

            return new RecognisePriorLearningResult
            {
                HasStandardOptions = apprenticeship.HasStandardOptions
            };
        }
    }
}