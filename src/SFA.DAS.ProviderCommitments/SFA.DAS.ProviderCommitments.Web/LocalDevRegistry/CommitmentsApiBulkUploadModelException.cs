//using Newtonsoft.Json;
//using SFA.DAS.CommitmentsV2.Api.Types.Responses;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SFA.DAS.CommitmentsV2.Api.Types.Validation
//{
//    public class CommitmentsApiBulkUploadModelException : Exception
//    {
//        public List<BulkUploadValidationError> Errors { get; }

//        public CommitmentsApiBulkUploadModelException(List<BulkUploadValidationError> errors) : base("Bulkupload Validation Exception")
//        {
//            Errors = errors;
//        }
//    }

//    public class BulkUploadErrorResponse
//    {
//        public IEnumerable<BulkUploadValidationError> DomainErrors { get; set; }

//        /// <summary>
//        /// Creates a Domain Exception with multiple domain errors
//        /// </summary>
//        /// <param name="errors"></param>
//        public BulkUploadErrorResponse(IEnumerable<BulkUploadValidationError> errors)
//        {
//            DomainErrors = errors;
//        }

//        public override string ToString()
//        {
//            return $"BulkUploadDomainException: {JsonConvert.SerializeObject(DomainErrors)}";
//        }
//    }
//}
