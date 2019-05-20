using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class NonReservationsAddDraftApprenticeshipRequestValidator : AbstractValidator<NonReservationsAddDraftApprenticeshipRequest>
    {
        public NonReservationsAddDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.CohortId).NotEmpty();
            RuleFor(model => model.CohortReference).NotEmpty();
        }
    }
}
