using FluentValidation;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload
{
    public class FileUploadValidateViewModelValidator : AbstractValidator<FileUploadValidateViewModel>
    {
        public FileUploadValidateViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration, IAuthorizationService authorizationService)
        {
            CascadeMode = CascadeMode.Stop;
            new FileUploadValidationHelper(csvConfiguration, authorizationService).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}
