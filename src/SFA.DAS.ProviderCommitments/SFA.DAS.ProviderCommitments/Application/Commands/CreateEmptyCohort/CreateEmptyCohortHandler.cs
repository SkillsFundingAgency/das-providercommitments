using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateEmptyCohort
{
    public class CreateEmptyCohortHandler : IRequestHandler<CreateEmptyCohortRequest, CreateEmptyCohortResponse>
    {
        private readonly IValidator<CreateEmptyCohortRequest> _validator;
        private readonly ICommitmentsApiClient _apiClient;
        private readonly IMapper<CreateEmptyCohortRequest, CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest> _mapper;

        public CreateEmptyCohortHandler(
            IValidator<CreateEmptyCohortRequest> validator,
            ICommitmentsApiClient apiClient,
            IMapper<CreateEmptyCohortRequest, CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest> mapper)
        {
            _validator = validator;
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<CreateEmptyCohortResponse> Handle(CreateEmptyCohortRequest request, CancellationToken cancellationToken)
        {
            ValidateAndThrow(request);

            var apiRequest = await _mapper.Map(request).ConfigureAwait(false);
            var apiResult = await _apiClient.CreateCohort(apiRequest, cancellationToken).ConfigureAwait(false);

            return new CreateEmptyCohortResponse
            {
                CohortId = apiResult.CohortId,
                CohortReference = apiResult.CohortReference
            };
        }

        private void ValidateAndThrow(CreateEmptyCohortRequest request)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
