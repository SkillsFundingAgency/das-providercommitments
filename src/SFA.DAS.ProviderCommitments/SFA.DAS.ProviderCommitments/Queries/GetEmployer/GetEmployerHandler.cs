using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerHandler : IRequestHandler<GetEmployerRequest, GetEmployerResponse>
    {
        private readonly IValidator<GetEmployerRequest> _validator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public GetEmployerHandler(IValidator<GetEmployerRequest> validator, ICommitmentsApiClient commitmentsApiClient)
        {
            _validator = validator;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<GetEmployerResponse> Handle(GetEmployerRequest request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);

            var legalEntity = await _commitmentsApiClient.GetLegalEntity(request.EmployerAccountLegalEntityId);

            return new GetEmployerResponse
            {
                AccountLegalEntityId = request.EmployerAccountLegalEntityId,
                AccountName = legalEntity.AccountName,
                LegalEntityName = legalEntity.LegalEntityName
            };
        }
    }
}