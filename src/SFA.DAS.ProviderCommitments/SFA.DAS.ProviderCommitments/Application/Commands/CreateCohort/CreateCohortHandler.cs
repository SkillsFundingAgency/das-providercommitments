using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortHandler : IRequestHandler<CreateCohortRequest, CreateCohortResponse>
    {
        private readonly IValidator<CreateCohortRequest> _validator;
        private readonly ICommitmentsApiClient _apiClient;
        private readonly IMapper<CreateCohortRequest, CommitmentsV2.Api.Types.Requests.CreateCohortRequest> _mapper;

        public CreateCohortHandler(
            IValidator<CreateCohortRequest> validator, 
            ICommitmentsApiClient apiClient,
            IMapper<CreateCohortRequest, CommitmentsV2.Api.Types.Requests.CreateCohortRequest> mapper)
        {
            _validator = validator;
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<CreateCohortResponse> Handle(CreateCohortRequest request, CancellationToken cancellationToken)
        {
            ValidateAndThrow(request);

            var apiRequest = await _mapper.Map(request).ConfigureAwait(false);
            var apiResult = await _apiClient.CreateCohort(apiRequest, cancellationToken).ConfigureAwait(false);

            return new CreateCohortResponse
            {
                CohortId = apiResult.CohortId,
                CohortReference = apiResult.CohortReference
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
