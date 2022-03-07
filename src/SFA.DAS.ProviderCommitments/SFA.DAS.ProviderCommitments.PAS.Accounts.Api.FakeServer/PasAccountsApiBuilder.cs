using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ProviderCommitments.PAS.Accounts.Api.FakeServer
{
    public class PasAccountsApiBuilder
    {
        private static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        private readonly WireMockServer _server;

        public PasAccountsApiBuilder(int port)
        {
            _server = WireMockServer.StartWithAdminInterface(port, true);
        }

        public static PasAccountsApiBuilder Create(int port)
        {
            return new PasAccountsApiBuilder(port);
        }

        public PasAccountsApi Build()
        {
            return new PasAccountsApi(_server);
        }

        public PasAccountsApiBuilder WithAgreementStatusSet()
        {
            var data = new 
            {
                Status = "Agreed"
            };
            var response = JsonConvert.SerializeObject(data, DefaultSerializerSettings);

            _server.Given(
                Request.Create()
                    .WithPath("/api/account/*/agreement")
                    .UsingGet()
                         )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(response)
                            );

            return this;
        }

    }
}
