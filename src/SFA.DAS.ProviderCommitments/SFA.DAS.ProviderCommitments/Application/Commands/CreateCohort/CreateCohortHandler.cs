using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortHandler : IRequestHandler<CreateCohortRequest, CreateCohortResponse>
    {
        private readonly IValidator<CreateCohortRequest> _validator;
        private readonly ICommitmentsApiClient _apiClient;
        private readonly IOuterApiService _outerApi;
        private readonly IMapper<CreateCohortRequest, CreateCohortApimRequest> _mapper;

        public CreateCohortHandler(
            IValidator<CreateCohortRequest> validator, 
            ICommitmentsApiClient apiClient,
            IOuterApiService outerApi,
            IMapper<CreateCohortRequest, CreateCohortApimRequest> mapper)
        {
            _validator = validator;
            _apiClient = apiClient;
            _outerApi = outerApi;
            _mapper = mapper;
        }

        public async Task<CreateCohortResponse> Handle(CreateCohortRequest request, CancellationToken cancellationToken)
        {
            ValidateAndThrow(request);

            var apiRequest = await _mapper.Map(request).ConfigureAwait(false);
            var apiResult = await _outerApi.CreateCohort(apiRequest).ConfigureAwait(false);

            var apprenticeships = await _apiClient.GetDraftApprenticeships(apiResult.CohortId, cancellationToken);

            long? draftApprenticeshipId = null;
            bool hasStandardOptions = false;
            if (apprenticeships.DraftApprenticeships.Count == 1)
            {
                var draftApprenticeship = await _apiClient.GetDraftApprenticeship(apiResult.CohortId,
                    apprenticeships.DraftApprenticeships.First().Id, cancellationToken);
                
                draftApprenticeshipId = draftApprenticeship.Id;
                hasStandardOptions = draftApprenticeship.HasStandardOptions;
            }
            
            return new CreateCohortResponse
            {
                CohortId = apiResult.CohortId,
                CohortReference = apiResult.CohortReference,
                HasStandardOptions = hasStandardOptions,
                DraftApprenticeshipId = draftApprenticeshipId,
            };
        }

        private void ValidateAndThrow(CreateCohortRequest request)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
