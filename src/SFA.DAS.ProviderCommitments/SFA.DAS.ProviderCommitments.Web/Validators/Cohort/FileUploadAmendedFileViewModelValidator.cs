using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class FileUploadAmendedFileViewModelValidator : AbstractValidator<FileUploadAmendedFileViewModel>
    {
        public FileUploadAmendedFileViewModelValidator()
        {
            RuleFor(x => x.Confirm).NotNull().WithMessage("Confirm if you want to upload a new file");
        }
    }
}
