using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class EditDraftApprenticeshipViewModelValidator : AbstractValidator<EditDraftApprenticeshipViewModel>
    {
        public EditDraftApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.DateOfBirth).Must(y => y.IsValid).WithMessage("The Date of birth is not valid").When(z => z.DateOfBirth.HasValue);
            RuleFor(x => x.StartDate).Must(y => y.IsValid).WithMessage("The start date is not valid").When(z => z.StartDate.HasValue);
            RuleFor(x => x.EndDate).Must(y => y.IsValid).WithMessage("The end date is not valid").When(z => z.EndDate.HasValue);
            RuleFor(x => x.ActualStartDate).Must(y => y.IsValid).WithMessage("The start date is not valid").When(z => z.ActualStartDate.HasValue);
        }
    }
}
