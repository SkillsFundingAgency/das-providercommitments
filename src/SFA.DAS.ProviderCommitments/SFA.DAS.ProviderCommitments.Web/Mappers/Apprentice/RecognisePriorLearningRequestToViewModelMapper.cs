using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

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
            //var apprenticeship = new GetDraftApprenticeshipResponse
            //{
            //    Cost = 0,
            //};

            var RecognisePriorLearningViewModel = new RecognisePriorLearningViewModel
            {
                IsTherePriorLearning = apprenticeship.Cost switch
                {
                    null => null,
                    0 => false,
                    _ => true,
                }
            };
            return RecognisePriorLearningViewModel;
        }
    }
}