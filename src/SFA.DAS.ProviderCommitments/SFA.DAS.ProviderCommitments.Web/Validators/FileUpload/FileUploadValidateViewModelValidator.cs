using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload
{
    public class FileUploadValidateViewModelValidator : FileUploadStartViewModelValidator
    {
        public FileUploadValidateViewModelValidator(ILogger<FileUploadStartViewModelValidator> logger, BulkUploadFileValidationConfiguration csvConfiguration) : base(logger, csvConfiguration)
        {
        }
    }
}
