using System;
using System.Net.Http;
using FluentValidation;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Authorisation;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using StructureMap;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    public class CommitmentsApiRegistry : Registry
    {
        public CommitmentsApiRegistry()
        {
            For<ICommitmentsApiClient>().Use<CommitmentsApiClient>("", ctx =>
            {
                // TODO: there is no authentication in the client yet.
                var config = ctx.GetInstance<CommitmentsClientApiConfiguration>();
                var httpClient = new HttpClient {BaseAddress = new Uri(config.BaseUrl)};
                var restClient = new CommitmentsRestHttpClient(httpClient);
                return new CommitmentsApiClient(restClient);
            }).Singleton();
            
        }
    }
}
