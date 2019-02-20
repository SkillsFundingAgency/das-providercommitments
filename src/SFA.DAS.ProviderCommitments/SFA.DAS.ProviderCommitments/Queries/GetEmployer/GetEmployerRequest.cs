using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerRequest : IRequest<GetEmployerResponse>
    {
        public string EmployerId { get; set; }
    }
}
