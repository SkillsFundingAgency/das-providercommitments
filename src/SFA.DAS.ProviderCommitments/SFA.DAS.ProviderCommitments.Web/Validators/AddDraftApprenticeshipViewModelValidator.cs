using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class AddDraftApprenticeshipViewModelValidator : AbstractValidator<AddDraftApprenticeshipViewModel>
    {
        public AddDraftApprenticeshipViewModelValidator()
        {
            //Example validation rules for testing
            //RuleFor(x => x.BirthDate).Must(y => y.IsValid).WithMessage("That date is not valid").When(z => z.BirthDate.HasValue);
            //RuleFor(x => x.StartDate).Must(y => y.IsValid).WithMessage("That date is not valid").When(z => z.StartDate.HasValue);
            //RuleFor(x => x.FinishDate).Must(y => y.IsValid).WithMessage("That date is not valid").When(z => z.FinishDate.HasValue);
        }
    }
}
