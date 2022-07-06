using FluentValidation;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
    {
        public FileUploadStartViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration, IAuthorizationService authorizationService)
        {
            CascadeMode = CascadeMode.Stop;

            new FileUploadValidationHelper(csvConfiguration, authorizationService).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}