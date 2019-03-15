using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortHandler : IRequestHandler<CreateCohortRequest, CreateCohortResponse>
    {
        private readonly IValidator<CreateCohortRequest> _validator;
        private readonly ICommitmentsApiClient _apiClient;

        public CreateCohortHandler(IValidator<CreateCohortRequest> validator, ICommitmentsApiClient apiClient)
        {
            _validator = validator;
            _apiClient = apiClient;
        }

        public async Task<CreateCohortResponse> Handle(CreateCohortRequest request, CancellationToken cancellationToken)
        {
            ValidateAndThrow(request);

            var apiResult = await _apiClient.CreateCohort(Map(request));

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

        private static CommitmentsV2.Api.Types.CreateCohortRequest Map(CreateCohortRequest source)
        {
            return new CommitmentsV2.Api.Types.CreateCohortRequest
            {  
                AccountLegalEntityId = source.AccountLegalEntityId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                DateOfBirth = source.DateOfBirth,
                ULN = source.UniqueLearnerNumber,
                CourseCode = source.CourseCode,
                Cost = source.Cost,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                OriginatorReference = source.OriginatorReference,
                ReservationId = source.ReservationId
            };
        }       
    }
}
