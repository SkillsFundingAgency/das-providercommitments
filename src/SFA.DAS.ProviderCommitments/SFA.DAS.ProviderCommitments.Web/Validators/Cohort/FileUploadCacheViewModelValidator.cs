using FluentValidation;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.Cohort
{
    public class FileUploadCacheViewModelValidator : AbstractValidator<FileUploadCacheViewModel>
    {
        public FileUploadCacheViewModelValidator()
        {
            RuleFor(x => x.SelectedOption).NotNull().WithMessage("Select how you want to add apprentices");
        }
    }
}
