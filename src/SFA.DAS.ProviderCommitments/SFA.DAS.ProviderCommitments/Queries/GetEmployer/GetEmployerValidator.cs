using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerValidator : AbstractValidator<GetEmployerRequest>
    {
        public GetEmployerValidator()
        {
            RuleFor(request => request.EmployerId).NotEmpty().WithMessage("Employer Id must be supplied and not be empty");
        }
    }
}
