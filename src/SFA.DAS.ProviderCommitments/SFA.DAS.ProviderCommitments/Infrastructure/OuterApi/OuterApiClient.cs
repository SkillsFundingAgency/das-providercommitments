using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi
{
    public class OuterApiClient : IOuterApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApprovalsOuterApiConfiguration _config;
        private ILogger<OuterApiClient> _logger;
        const string SubscriptionKeyRequestHeaderKey = "Ocp-Apim-Subscription-Key";
        const string VersionRequestHeaderKey = "X-Version";

        public OuterApiClient(HttpClient httpClient, ApprovalsOuterApiConfiguration config, ILogger<OuterApiClient> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config.ApiBaseUrl);
            _logger = logger;
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {
            AddHeaders();

            var response = await _httpClient.GetAsync(request.GetUrl).ConfigureAwait(false);

            if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                return default;
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResponse>(json);
            }

            response.EnsureSuccessStatusCode();
            return default;
        }

        private void AddHeaders()
        {
            //The http handler life time is set to 5 minutes
            //hence once the headers are added they don't need added again
            if (_httpClient.DefaultRequestHeaders.Contains(SubscriptionKeyRequestHeaderKey)) return;

            _httpClient.DefaultRequestHeaders.Add(SubscriptionKeyRequestHeaderKey, _config.SubscriptionKey);
            _httpClient.DefaultRequestHeaders.Add(VersionRequestHeaderKey, "1");
        }

        public async Task<TResponse> Post<TResponse>(IPostApiRequest request)
        {
            return await PutOrPost<TResponse>(request.Data, HttpMethod.Post, request.PostUrl);
        }

        public async Task<TResponse> Put<TResponse>(IPutApiRequest request)
        {
            return await PutOrPost<TResponse>(request.Data, HttpMethod.Put, request.PutUrl);
        }

        private async Task<TResponse> PutOrPost<TResponse>(object data, HttpMethod method, string url)
        {
            AddHeaders();
            var stringContent = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json")
                : null;

            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = stringContent;

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // var errorContent = "";
            var responseBody = (TResponse) default;


            if (IsNot200RangeResponseCode(response.StatusCode))
            {
                //Plug this in when moving another Post endpoint which throws domain errors
                if (response.StatusCode == HttpStatusCode.BadRequest &&
                    response.GetSubStatusCode() == HttpSubStatusCode.DomainException)
                {
                    throw CreateApiModelException(response, json);
                }

                if (response.StatusCode == HttpStatusCode.BadRequest &&
                    response.GetSubStatusCode() == HttpSubStatusCode.BulkUploadDomainException)
                {
                    throw CreateBulkUploadApiModelException(response, json);
                }
                else
                {
                    throw new RestHttpClientException(response, json);
                }
            }
            else
            {
                responseBody = JsonConvert.DeserializeObject<TResponse>(json);
            }

            return responseBody;
        }

        private static bool IsNot200RangeResponseCode(HttpStatusCode statusCode)
        {
            return !((int)statusCode >= 200 && (int)statusCode <= 299);
        }

        private Exception CreateBulkUploadApiModelException(HttpResponseMessage httpResponseMessage, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"{httpResponseMessage.RequestMessage.RequestUri} has returned an empty string when an array of error responses was expected.");
                return new CommitmentsApiBulkUploadModelException(new List<BulkUploadValidationError>());
            }

            var errors = new CommitmentsApiBulkUploadModelException(JsonConvert.DeserializeObject<BulkUploadErrorResponse>(content).DomainErrors?.ToList());
            return errors;
        }

        private Exception CreateApiModelException(HttpResponseMessage httpResponseMessage, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"{httpResponseMessage.RequestMessage.RequestUri} has returned an empty string when an array of error responses was expected.");
                return new CommitmentsV2.Api.Types.Validation.CommitmentsApiModelException(new List<CommitmentsV2.Api.Types.Validation.ErrorDetail>());
            }

            var errors = new CommitmentsV2.Api.Types.Validation.CommitmentsApiModelException(JsonConvert.DeserializeObject<CommitmentsV2.Api.Types.Validation.ErrorResponse>(content).Errors);

            var errorDetails = string.Join(";", errors.Errors.Select(e => $"{e.Field} ({e.Message})"));
            _logger.Log(errors.Errors.Count == 0 ? LogLevel.Warning : LogLevel.Debug, $"{httpResponseMessage.RequestMessage.RequestUri} has returned {errors.Errors.Count} errors: {errorDetails}");

            return errors;
        }

    }



    public interface IOuterApiClient
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
        Task<TResponse> Post<TResponse>(IPostApiRequest request);
        Task<TResponse> Put<TResponse>(IPutApiRequest request);
    }
}
