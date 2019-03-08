using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityValidator : AbstractValidator<GetAccountLegalEntityRequest>
    {
        public GetAccountLegalEntityValidator()
        {
            RuleFor(request => request.EmployerAccountLegalEntityId).GreaterThan(0).WithMessage(request => $"{nameof(request.EmployerAccountLegalEntityId)} must be supplied and be a positive integer");
        }
    }
}
