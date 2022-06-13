﻿using FluentValidation;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
    {
        public FileUploadStartViewModelValidator(BulkUploadFileValidationConfiguration csvConfiguration)
        {
            CascadeMode = CascadeMode.Stop;

            new FileUploadValidationHelper(csvConfiguration).AddFileValidationRules(RuleFor(x => x.Attachment));
        }
    }
}