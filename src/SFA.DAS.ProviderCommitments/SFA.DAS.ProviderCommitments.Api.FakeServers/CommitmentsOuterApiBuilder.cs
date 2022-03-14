using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers
{
    public class CommitmentsOuterApiBuilder
    {
        private readonly WireMockServer _server;

        public CommitmentsOuterApiBuilder(int port)
        {
            _server = WireMockServer.StartWithAdminInterface(port, true);
        }

        public static CommitmentsOuterApiBuilder Create(int port)
        {
            return new CommitmentsOuterApiBuilder(port);
        }

        public MockApi Build()
        {
            return new MockApi(_server);
        }

        internal CommitmentsOuterApiBuilder WithCourseDeliveryModels()
        {
            _server
                .Given(Request.Create()
                    .WithPath("/providers/*/courses/*")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new
                    {
                        DeliveryModels = new[] { "Normal", "Flexible" },
                    }));

            _server
                .Given(Request.Create()
                    .WithPath("/providers/*/courses/650")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new
                    {
                        DeliveryModels = new[] { "Normal", "Flexible" },
                    }));

            return this;
        }
    }
}