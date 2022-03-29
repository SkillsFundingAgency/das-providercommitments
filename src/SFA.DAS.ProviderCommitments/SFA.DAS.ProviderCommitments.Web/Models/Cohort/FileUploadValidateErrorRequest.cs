using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class FileUploadValidateErrorRequest
    {
        private List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError> errors;
        public long ProviderId { get; set; }
        public List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError> Errors 
        {
            get => errors;
            set
            {
                errors = value;
                if (errors == null)
                    errors = new List<CommitmentsV2.Api.Types.Responses.BulkUploadValidationError>();
            }
        }

    }
}
