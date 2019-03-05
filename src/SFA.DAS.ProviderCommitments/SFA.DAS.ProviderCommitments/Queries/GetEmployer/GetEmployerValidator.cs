using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerValidator : AbstractValidator<GetEmployerRequest>
    {
        public GetEmployerValidator()
        {
            RuleFor(request => request.EmployerAccountLegalEntityId).NotEmpty().WithMessage(request => $"{nameof(request.EmployerAccountLegalEntityId)} must be supplied and not be empty");
        }
    }
}
