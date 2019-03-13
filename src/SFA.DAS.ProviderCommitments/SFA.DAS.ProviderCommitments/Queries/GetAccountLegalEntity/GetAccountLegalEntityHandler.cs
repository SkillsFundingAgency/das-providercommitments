using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;

namespace SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityHandler : IRequestHandler<GetAccountLegalEntityRequest, GetAccountLegalEntityResponse>
    {
        private readonly IValidator<GetAccountLegalEntityRequest> _validator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public GetAccountLegalEntityHandler(IValidator<GetAccountLegalEntityRequest> validator, ICommitmentsApiClient commitmentsApiClient)
        {
            _validator = validator;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<GetAccountLegalEntityResponse> Handle(GetAccountLegalEntityRequest request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);
            var legalEntity = await _commitmentsApiClient.GetLegalEntity(request.EmployerAccountLegalEntityId);

            return new GetAccountLegalEntityResponse
            {
                AccountLegalEntityId = request.EmployerAccountLegalEntityId,
                AccountName = legalEntity.AccountName,
                LegalEntityName = legalEntity.LegalEntityName
            };
        }
    }
}