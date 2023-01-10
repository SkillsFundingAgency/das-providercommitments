using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers
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

        public MockApi Build()
        {
            Console.WriteLine($"PasAccounts Fake Api Running ({_server.Urls[0]})");
            return new MockApi(_server);
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