using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityRequest : IRequest<GetAccountLegalEntityResponse>
    {
        public long EmployerAccountLegalEntityId { get; set; }
    }
}
