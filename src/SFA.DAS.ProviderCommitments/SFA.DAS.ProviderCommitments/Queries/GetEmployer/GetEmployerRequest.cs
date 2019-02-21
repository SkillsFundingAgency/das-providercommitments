using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerRequest : IRequest<GetEmployerResponse>
    {
        public string EmployerAccountPublicHashedId { get; set; }
        public string EmployerAccountLegalEntityPublicHashedId { get; set; }
    }
}
