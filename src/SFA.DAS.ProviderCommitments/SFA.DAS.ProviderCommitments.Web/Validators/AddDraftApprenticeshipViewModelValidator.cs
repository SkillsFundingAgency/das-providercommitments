using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class AddDraftApprenticeshipViewModelValidator : AbstractValidator<AddDraftApprenticeshipViewModel>
    {
        public AddDraftApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.BirthDate).Must(y => y.IsValid).WithMessage("Date of birth is invalid").When(z => z.BirthDate.HasValue);
            RuleFor(x => x.StartDate).Must(y => y.IsValid).WithMessage("Planned training start date is invalid").When(z => z.StartDate.HasValue);
            RuleFor(x => x.FinishDate).Must(y => y.IsValid).WithMessage("Projected finish date is invalid").When(z => z.FinishDate.HasValue);
        }
    }
}
