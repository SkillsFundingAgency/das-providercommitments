using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling
{
    public class BulkUploadErrorResponse
    {
        public IEnumerable<BulkUploadValidationError> DomainErrors { get; set; }

        /// <summary>
        /// Creates a Domain Exception with multiple domain errors
        /// </summary>
        /// <param name="errors"></param>
        public BulkUploadErrorResponse(IEnumerable<BulkUploadValidationError> errors)
        {
            DomainErrors = errors;
        }

        public override string ToString()
        {
            return $"BulkUploadDomainException: {JsonConvert.SerializeObject(DomainErrors)}";
        }
    }
}
