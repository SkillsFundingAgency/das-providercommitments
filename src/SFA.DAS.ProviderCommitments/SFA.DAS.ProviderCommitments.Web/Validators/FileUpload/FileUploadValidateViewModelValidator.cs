using FluentValidation;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

public class FileUploadValidateViewModelValidator : AbstractValidator<FileUploadValidateViewModel>
{
    public FileUploadValidateViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
    {
        new FileUploadValidationHelper(csvConfiguration).AddFileValidationRules(RuleFor(x => x.Attachment));
    }
}