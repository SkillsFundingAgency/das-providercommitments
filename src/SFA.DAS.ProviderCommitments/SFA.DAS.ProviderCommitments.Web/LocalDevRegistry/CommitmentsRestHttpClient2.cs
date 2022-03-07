using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.ProviderCommitments.Web.LocalDevRegistry
{
    public class CommitmentsRestHttpClient2 : CommitmentsRestHttpClient
    {
        public CommitmentsRestHttpClient2(HttpClient httpClient, ILoggerFactory loggerFactory) : base(httpClient, loggerFactory)
        {
        }

        protected override Exception CreateClientException(System.Net.Http.HttpResponseMessage httpResponseMessage, string content)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest && httpResponseMessage.GetSubStatusCode() == HttpSubStatusCode.DomainException)
            {
                return CreateApiModelException(httpResponseMessage, content);
            }
            else if (1 == 1)
            {
                return CreateBulkUploadApiModelException(httpResponseMessage, content);
            }
            else
            {
                return base.CreateClientException(httpResponseMessage, content);
            }
        }

        private Exception CreateApiModelException(HttpResponseMessage httpResponseMessage, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new CommitmentsApiModelException(new List<ErrorDetail>());
            }

            var errors = new CommitmentsApiModelException(JsonConvert.DeserializeObject<ErrorResponse>(content).Errors);

            var errorDetails = string.Join(";", errors.Errors.Select(e => $"{e.Field} ({e.Message})"));
            return errors;
        }

        private Exception CreateBulkUploadApiModelException(HttpResponseMessage httpResponseMessage, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new CommitmentsApiBulkUploadModelException(new List<BulkUploadValidationError>());
            }

            var errors = new CommitmentsApiBulkUploadModelException(JsonConvert.DeserializeObject<BulkUploadErrorResponse>(content).DomainErrors?.ToList());

            return errors;
        }
    }
}
