using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class NonReservationsAddDraftApprenticeshipRequestValidator : AbstractValidator<NonReservationsAddDraftApprenticeshipRequest>
    {
        public NonReservationsAddDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ProviderId).NotEmpty();
            RuleFor(model => model.CohortId).NotEmpty();
            RuleFor(model => model.CohortReference).NotEmpty();
        }
    }
}
