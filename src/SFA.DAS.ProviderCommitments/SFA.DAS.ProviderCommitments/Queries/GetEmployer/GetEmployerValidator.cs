using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetEmployer
{
    public class GetEmployerValidator : AbstractValidator<GetEmployerRequest>
    {
        public GetEmployerValidator()
        {
            RuleFor(request => request.EmployerAccountLegalEntityId).GreaterThan(0).WithMessage(request => $"{nameof(request.EmployerAccountLegalEntityId)} must be supplied and be a positive integer");
        }
    }
}
