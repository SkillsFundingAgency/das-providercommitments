using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortHandler : IRequestHandler<CreateCohortRequest, CreateCohortResponse>
    {
        public Task<CreateCohortResponse> Handle(CreateCohortRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
