using CsvHelper.Configuration;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload
{
    public class FileUploadValidateViewModelValidator : AbstractValidator<FileUploadValidateViewModel>
    {
        private readonly DAS.Authorization.Services.IAuthorizationService _authorizationService;

        public FileUploadValidateViewModelValidator(DAS.Authorization.Services.IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public FileUploadValidateViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
        {
            CascadeMode = CascadeMode.Stop;
            new FileUploadValidationHelper(csvConfiguration, _authorizationService).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}
