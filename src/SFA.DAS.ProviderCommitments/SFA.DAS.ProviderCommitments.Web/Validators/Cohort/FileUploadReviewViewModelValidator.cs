using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class FileUploadReviewViewModelValidator : AbstractValidator<FileUploadReviewViewModel>
    {
        public FileUploadReviewViewModelValidator()
        {
            RuleFor(x => x.SelectedOption).NotNull().WithMessage("You need to choose an option");
        }
    }
}
