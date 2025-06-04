using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class RecognisePriorLearningRequestToViewModelMapper : IMapper<RecognisePriorLearningRequest, RecognisePriorLearningViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public RecognisePriorLearningRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<RecognisePriorLearningViewModel> Map(RecognisePriorLearningRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);

            return new RecognisePriorLearningViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                IsTherePriorLearning = apprenticeship.RecognisePriorLearning,
                RplNeedsToBeConsidered = RecognisePriorLearningHelper.DoesDraftApprenticeshipRequireRpl(apprenticeship.ActualStartDate, apprenticeship.StartDate)
            };
        }
    }
}