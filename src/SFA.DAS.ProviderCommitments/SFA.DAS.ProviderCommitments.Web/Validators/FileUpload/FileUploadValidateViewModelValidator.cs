using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload
{
    public class FileUploadValidateViewModelValidator : AbstractValidator<FileUploadValidateViewModel>
    {
        private FileUploadStartViewModelValidator _validator;

        public FileUploadValidateViewModelValidator(FileUploadStartViewModelValidator validator) 
        {
            _validator = validator;
        }

        public override ValidationResult Validate(ValidationContext<FileUploadValidateViewModel> context)
        {
            return _validator.Validate(context.InstanceToValidate);
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<FileUploadValidateViewModel> context, CancellationToken cancellation = default)
        {
            return _validator.ValidateAsync(context.InstanceToValidate, cancellation);
        }
    }
}
