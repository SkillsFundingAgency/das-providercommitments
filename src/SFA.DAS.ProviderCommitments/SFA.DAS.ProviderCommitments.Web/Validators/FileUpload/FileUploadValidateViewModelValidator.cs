using FluentValidation;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload
{
    public class FileUploadValidateViewModelValidator : AbstractValidator<FileUploadValidateViewModel>
    {
        public FileUploadValidateViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
        {
            CascadeMode = CascadeMode.Stop;
            new FileUploadValidationHelper(csvConfiguration).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}
