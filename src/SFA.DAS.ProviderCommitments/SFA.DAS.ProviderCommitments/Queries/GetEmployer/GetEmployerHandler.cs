using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerHandler : IRequestHandler<GetEmployerRequest, GetEmployerResponse>
    {
        private readonly IValidator<GetEmployerRequest> _validator;

        public GetEmployerHandler(IValidator<GetEmployerRequest> validator)
        {
            _validator = validator;
        }

        public Task<GetEmployerResponse> Handle(GetEmployerRequest request, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(request);

            return Task.FromResult(new GetEmployerResponse
            {
                EmployerId = request.EmployerId,
                EmployerName = "** Temp Place Holder **"
            });
        }
    }
}