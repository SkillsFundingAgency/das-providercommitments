using System;
using System.Collections.Generic;
using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers
{
    public class CommitmentsOuterApiBuilder
    {
        private readonly WireMockServer _server;

        public CommitmentsOuterApiBuilder(int port)
        {
            _server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = port,
                UseSSL = true,
                StartAdminInterface = true,
                Logger = new WireMockConsoleLogger(),
            });
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
                    .WithPath("/approvals/providers/*/courses*")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new
                    {
                        DeliveryModels = new[] { "Regular", "PortableFlexiJob" },
                    }));

            return this;
        }

        internal CommitmentsOuterApiBuilder WithBulkUpload()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath("*")
                        .UsingAnyMethod())
                    .AtPriority(10)
                .RespondWith(
                    Response.Create()
                        .WithProxy("https://localhost:5011/api"));

            _server
                .Given(Request.Create()
                    .WithPath("/BulkUpload/Validate")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));

            _server.Given(Request.Create()
                .WithPath("/Cohort/*")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new { LatestMessageCreatedByProvider = "No message from provider" }));

            _server.Given(Request.Create()
                .WithPath("/TrainingCourses/standards/*")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new
                { 
                    Title = "",
                    EffectiveTo = DateTime.UtcNow,
                    EffectiveFrom = DateTime.UtcNow,
                    ApprenticeshipFunding = new object[0]
                }));

            return this;
        }
    }
}