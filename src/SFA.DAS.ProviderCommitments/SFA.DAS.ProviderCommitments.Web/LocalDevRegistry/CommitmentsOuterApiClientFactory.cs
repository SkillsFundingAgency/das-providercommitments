using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web
{
    public class LocalDevOuterApiClientFactory : ICommitmentsOuterApiHttpClientFactory, ICommitmentsOuterApiHttpClient
    {
        public CommitmentsOuterApiClient CreateClient()
        {
            return new CommitmentsOuterApiClient(this);
        }

        public Task<T> Get<T>(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
            => Get<T>(uri.ToString(), queryData, cancellationToken);

        public async Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            var models = new CourseDeliveryModels
            {
                DeliveryModels = new[]
                {
                    DeliveryModel.Normal,
                    DeliveryModel.Flexible,
                }
            };

            if (models is T t)
            {
                return t;
            }

            try
            {
                return (T)Convert.ChangeType(models, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        public Task<string> Get(string uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> Get(Uri uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PostAsJson<TResponse>(string uri, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> PostAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PostAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> PutAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PutAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}