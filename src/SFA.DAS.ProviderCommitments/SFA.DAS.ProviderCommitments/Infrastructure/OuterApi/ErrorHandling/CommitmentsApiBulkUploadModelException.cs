﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling
{
    public class CommitmentsApiBulkUploadModelException : Exception
    {
        public List<BulkUploadValidationError> Errors { get; }

        public CommitmentsApiBulkUploadModelException(List<BulkUploadValidationError> errors) : base("Bulkupload Validation Exception")
        {
            Errors = errors;
        }
    }
}