using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

public interface IOuterApiClient
{
    Task<TResponse> Get<TResponse>(IGetApiRequest request);
    Task<TResponse> Post<TResponse>(IPostApiRequest request);
    Task<TResponse> Put<TResponse>(IPutApiRequest request);
}

public class OuterApiClient : IOuterApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ApprovalsOuterApiConfiguration _config;
    private readonly ILogger<OuterApiClient> _logger;
    private const string SubscriptionKeyRequestHeaderKey = "Ocp-Apim-Subscription-Key";
    private const string VersionRequestHeaderKey = "X-Version";

    public OuterApiClient(
        IHttpClientFactory httpClientFactory,
        ApprovalsOuterApiConfiguration config,
        ILogger<OuterApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _config = config;
        _httpClient.BaseAddress = new Uri(_config.ApiBaseUrl);
        _logger = logger;
    }

    public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        
        AddAuthenticationHeaders(requestMessage);

        using var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

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

    private void AddAuthenticationHeaders(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.Add(SubscriptionKeyRequestHeaderKey, _config.SubscriptionKey);
        httpRequestMessage.Headers.Add(VersionRequestHeaderKey, "1");
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
        var requestMessage = new HttpRequestMessage(method, url);
        AddAuthenticationHeaders(requestMessage);

        var stringContent = data != null
            ? new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json")
            : null;

        requestMessage.Content = stringContent;

        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var responseBody = (TResponse)default;

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

            throw new RestHttpClientException(response, json);
        }

        responseBody = JsonConvert.DeserializeObject<TResponse>(json);

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
            _logger.LogWarning("{RequestUri} has returned an empty string when an array of error responses was expected.", httpResponseMessage.RequestMessage.RequestUri);
            return new CommitmentsApiBulkUploadModelException([]);
        }

        return new CommitmentsApiBulkUploadModelException(JsonConvert.DeserializeObject<BulkUploadErrorResponse>(content).DomainErrors?.ToList());
    }

    private Exception CreateApiModelException(HttpResponseMessage httpResponseMessage, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogWarning("{RequestUri} has returned an empty string when an array of error responses was expected.", httpResponseMessage.RequestMessage.RequestUri);
            return new CommitmentsV2.Api.Types.Validation.CommitmentsApiModelException([]);
        }

        var errors = new CommitmentsV2.Api.Types.Validation.CommitmentsApiModelException(JsonConvert.DeserializeObject<CommitmentsV2.Api.Types.Validation.ErrorResponse>(content).Errors);

        var errorDetails = string.Join(";", errors.Errors.Select(e => $"{e.Field} ({e.Message})"));
        
        _logger.Log(errors.Errors.Count == 0 ? LogLevel.Warning : LogLevel.Debug, "{RequestUri} has returned {ErrorsCount} errors: {ErrorDetails}", httpResponseMessage.RequestMessage.RequestUri, errors.Errors.Count, errorDetails);

        return errors;
    }
}