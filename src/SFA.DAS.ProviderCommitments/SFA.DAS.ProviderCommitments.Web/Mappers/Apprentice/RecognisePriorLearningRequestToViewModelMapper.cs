using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Helpers;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class RecognisePriorLearningRequestToViewModelMapper(
        ICommitmentsApiClient commitmentsApiClient,
        IOuterApiService outerApiService)
        : IMapper<RecognisePriorLearningRequest, RecognisePriorLearningViewModel>
    {
        public async Task<RecognisePriorLearningViewModel> Map(RecognisePriorLearningRequest source)
        {
            var apprenticeship = await commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            
            var isRplRequired = true;
            if (!string.IsNullOrEmpty(apprenticeship.CourseCode))
            {
                var rplRequirements = await outerApiService.GetRplRequirements(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, apprenticeship.CourseCode);
                isRplRequired = rplRequirements?.IsRequired ?? true;
            }
            
            return new RecognisePriorLearningViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                ProviderId = source.ProviderId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                IsTherePriorLearning = apprenticeship.RecognisePriorLearning,
                IsRplRequired = isRplRequired,
                RplNeedsToBeConsidered = RecognisePriorLearningHelper.DoesDraftApprenticeshipRequireRpl(apprenticeship.ActualStartDate, apprenticeship.StartDate)
            };
        }
    }
}