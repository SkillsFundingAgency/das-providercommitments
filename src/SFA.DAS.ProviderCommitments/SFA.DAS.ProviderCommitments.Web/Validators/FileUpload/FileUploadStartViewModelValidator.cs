using FluentValidation;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
    {
        private readonly DAS.Authorization.Services.IAuthorizationService _authorizationService;

        public FileUploadStartViewModelValidator(DAS.Authorization.Services.IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public FileUploadStartViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
        {
            CascadeMode = CascadeMode.Stop;

            new FileUploadValidationHelper(csvConfiguration, _authorizationService).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}