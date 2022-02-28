using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadValidateViewModel
    {
        public long ProviderId { get; set; }
        public FileUploadValidateViewModel()
        {
            BulkUploadValidationErrors = new List<BulkUploadValidationError>();
        }
        public List<BulkUploadValidationError> BulkUploadValidationErrors { get; set; }
    }

    public class BulkUploadValidationError
    {
        public BulkUploadValidationError() { PropertyErrors = new List<PropertyError>(); }
        public BulkUploadValidationError(int rowNumber, string employerName, string uLN, string apprenticeName, List<PropertyError> errors)
        {
            RowNumber = rowNumber;
            EmployerName = employerName;
            Uln = uLN;
            ApprenticeName = apprenticeName;
            PropertyErrors = errors;
        }

        public int RowNumber { get; set; }
        public string EmployerName { get; set; }
        public string Uln { get; set; }
        public string ApprenticeName { get; set; }
        public List<PropertyError> PropertyErrors { get; set; }
    }

    public class PropertyError
    {
        public PropertyError() { }

        public PropertyError(string property, string error)
        {
            Property = property;
            ErrorText = error;
        }

        public string Property { get; set; }
        public string ErrorText { get; set; }
    }
}
