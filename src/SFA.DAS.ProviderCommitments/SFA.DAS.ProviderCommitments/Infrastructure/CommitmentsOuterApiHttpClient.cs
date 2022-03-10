﻿using SFA.DAS.ProviderCommitments.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CommitmentsOuterApiHttpClient : ICommitmentsOuterApiHttpClient
    {
        public Task<string> Get(Uri uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> Get(string uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<T> Get<T>(Uri uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PostAsJson<TResponse>(string uri, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> PostAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PostAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<string> PutAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<TResponse> PutAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

}