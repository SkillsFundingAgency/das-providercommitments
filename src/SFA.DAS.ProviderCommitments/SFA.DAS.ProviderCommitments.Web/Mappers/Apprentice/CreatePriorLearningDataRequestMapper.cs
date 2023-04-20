using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class CreatePriorLearningDataRequestMapper : IMapper<PriorLearningDataViewModel, CreatePriorLearningDataApimRequest>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ITempDataStorageService _storageService;

        public CreatePriorLearningDataRequestMapper(IOuterApiClient outerApiClient, ITempDataStorageService storageService)
        {
            _outerApiClient = outerApiClient;
            _storageService = storageService;
        }

        public async Task<CreatePriorLearningDataApimRequest> Map(PriorLearningDataViewModel source)
        {
            var cachedModel = _storageService.RetrieveFromCache<EditDraftApprenticeshipViewModel>();

            var apprenticeshipRequest = new GetEditDraftApprenticeshipRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, cachedModel?.CourseCode);
            var apprenticeshipResponse = await _outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apprenticeshipRequest);

            return await Task.FromResult(new CreatePriorLearningDataApimRequest
            {
                DurationReducedBy = source.DurationReducedBy,
                CostBeforeRpl = source.CostBeforeRpl,
                DurationReducedByHours = source.DurationReducedByHours, 
                IsDurationReducedByRpl = source.IsDurationReducedByRpl,
                PriceReducedBy = source.PriceReduced,
                TrainingTotalHours= source.TrainingTotalHours,
                HasStandardOptions = apprenticeshipResponse.HasStandardOptions
            });
        }
    }
}