﻿using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling
{
    public class BulkUploadValidationError
    {
        public BulkUploadValidationError(int rowNumber, string employerName, string uLN, string apprenticeName, List<Error> errors)
        {
            RowNumber = rowNumber;
            EmployerName = employerName;
            Uln = uLN;
            ApprenticeName = apprenticeName;
            Errors = errors;
        }

        public int RowNumber { get; set; }
        public string EmployerName { get; set; }
        public string Uln { get; set; }
        public string ApprenticeName { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public Error() { }
        public Error(string property, string error)
        {
            Property = property;
            ErrorText = error;
        }

        public string Property { get; set; }
        public string ErrorText { get; set; }
    }
}
