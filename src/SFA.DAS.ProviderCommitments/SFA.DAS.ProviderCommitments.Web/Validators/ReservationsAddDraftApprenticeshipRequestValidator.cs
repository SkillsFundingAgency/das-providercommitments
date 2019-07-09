using FluentValidation;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class ReservationsAddDraftApprenticeshipRequestValidator : AbstractValidator<ReservationsAddDraftApprenticeshipRequest>
    {
        public ReservationsAddDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ProviderId).NotEmpty();
            RuleFor(model => model.CohortId).NotEmpty();
            RuleFor(model => model.CohortReference).NotEmpty();
            RuleFor(model => model.StartMonthYear)
                .Must(monthYear => new MonthYearModel(monthYear).IsValid)
                .When(model => !string.IsNullOrWhiteSpace(model.StartMonthYear))
                .WithMessage("{PropertyName} is invalid");
        }
    }
}
