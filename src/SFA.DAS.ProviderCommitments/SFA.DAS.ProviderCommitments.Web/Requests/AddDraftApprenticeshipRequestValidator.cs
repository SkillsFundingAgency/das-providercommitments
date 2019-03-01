using FluentValidation;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class AddDraftApprenticeshipRequestValidator : AbstractValidator<AddDraftApprenticeshipRequest>
    {
        public AddDraftApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ReservationId).NotEmpty();
            RuleFor(model => model.EmployerAccountPublicHashedId).NotEmpty();
            RuleFor(model => model.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(model => model.StartMonthYear)
                .Must(monthYear => new MonthYearModel(monthYear).IsValid)
                .When(model => !string.IsNullOrWhiteSpace(model.StartMonthYear))
                .WithMessage("{PropertyName} is invalid");
        }
    }
}
