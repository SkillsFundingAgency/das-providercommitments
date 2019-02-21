using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerValidator : AbstractValidator<GetEmployerRequest>
    {
        public GetEmployerValidator()
        {
            RuleFor(request => request.EmployerAccountPublicHashedId).NotEmpty().WithMessage("EmployerAccountPublicHashedId must be supplied and not be empty");
        }
    }
}
