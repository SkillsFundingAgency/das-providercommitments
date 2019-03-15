using FluentValidation;

namespace SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort
{
    public class CreateCohortValidator : AbstractValidator<CreateCohortRequest>
    {
        public CreateCohortValidator()
        {
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.AccountLegalEntityId).NotEmpty();
            RuleFor(x => x.ReservationId).NotEmpty();
        }
    }
}
