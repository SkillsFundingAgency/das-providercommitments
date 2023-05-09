using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class RecognisePriorLearningViewModelToResultMapper : IMapper<RecognisePriorLearningViewModel, RecognisePriorLearningResult>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiClient _outerApiClient;

        public RecognisePriorLearningViewModelToResultMapper(ICommitmentsApiClient commitmentsApiClient, IOuterApiClient outerApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _outerApiClient = outerApiClient;

        }

        public async Task<RecognisePriorLearningResult> Map(RecognisePriorLearningViewModel source)
        {
            var update = _commitmentsApiClient.RecognisePriorLearning(
                source.CohortId,
                source.DraftApprenticeshipId,
                new CommitmentsV2.Api.Types.Requests.RecognisePriorLearningRequest
                {
                    RecognisePriorLearning = source.IsTherePriorLearning
                });

            var request = new GetEditDraftApprenticeshipRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, string.Empty);
            var apprenticeship = await _outerApiClient.Get<GetEditDraftApprenticeshipResponse>(request);

            return new RecognisePriorLearningResult
            {
                HasStandardOptions = apprenticeship.HasStandardOptions
            };
        }
    }
    
    public class PriorLearningDetailsViewModelToResultMapper : IMapper<PriorLearningDetailsViewModel, RecognisePriorLearningResult>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IOuterApiClient _outerApiClient;
        private readonly DAS.Authorization.Services.IAuthorizationService _authorizationService;

        public PriorLearningDetailsViewModelToResultMapper(ICommitmentsApiClient commitmentsApiClient, DAS.Authorization.Services.IAuthorizationService authorizationService, IOuterApiClient outerApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _authorizationService = authorizationService;
            _outerApiClient = outerApiClient;
        }

        public async Task<RecognisePriorLearningResult> Map(PriorLearningDetailsViewModel source)
        {
            var update = _commitmentsApiClient.PriorLearningDetails(
                source.CohortId,
                source.DraftApprenticeshipId,
                new CommitmentsV2.Api.Types.Requests.PriorLearningDetailsRequest
                {
                    DurationReducedBy = source.ReducedDuration,
                    PriceReducedBy = source.ReducedPrice,
                    DurationReducedByHours = source.DurationReducedByHours,
                    WeightageReducedBy = source.WeightageReducedBy,
                    QualificationsForRplReduction = source.QualificationsForRplReduction,
                    ReasonForRplReduction = source.ReasonForRplReduction,
                    Rpl2Mode = await _authorizationService.IsAuthorizedAsync(ProviderFeature.RplExtended)
                });

            var request = new GetEditDraftApprenticeshipRequest(source.ProviderId, source.CohortId, source.DraftApprenticeshipId, string.Empty);
            var apprenticeship = await _outerApiClient.Get<GetEditDraftApprenticeshipResponse>(request);

            return new RecognisePriorLearningResult
            {
                HasStandardOptions = apprenticeship.HasStandardOptions
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

            return new RecognisePriorLearningResult
            {
                HasStandardOptions = update.HasStandardOptions,
                RplPriceReductionError = update.RplPriceReductionError
            };
        }
    }
}