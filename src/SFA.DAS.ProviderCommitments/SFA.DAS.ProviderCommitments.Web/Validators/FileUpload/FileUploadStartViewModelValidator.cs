using FluentValidation;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
{
    public FileUploadStartViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
    {
        new FileUploadValidationHelper(csvConfiguration).AddFileValidationRules(RuleFor(x => x.Attachment));
    }
}