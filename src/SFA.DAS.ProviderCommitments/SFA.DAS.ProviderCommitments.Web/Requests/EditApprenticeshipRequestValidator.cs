using FluentValidation;
using SFA.DAS.ProviderCommitments.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class EditApprenticeshipRequestValidator : AbstractValidator<EditApprenticeshipRequest>
    {
        public EditApprenticeshipRequestValidator()
        {
            RuleFor(model => model.ReservationId).NotEmpty();
            RuleFor(model => model.EmployerAccountPublicHashedId).NotEmpty();
            RuleFor(model => model.EmployerAccountLegalEntityPublicHashedId).NotEmpty();
            RuleFor(model => model.StartMonthYear).NotNull();
            RuleFor(model => model.StartMonthYear).Must(monthYear => new MonthYearModel(monthYear).IsValid).WithMessage("{PropertyName} is invalid");
        }
    }
}
