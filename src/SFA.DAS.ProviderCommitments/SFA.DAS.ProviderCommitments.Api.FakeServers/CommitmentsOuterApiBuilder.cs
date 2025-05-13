using System;
using System.Collections.Generic;
using System.Net;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace SFA.DAS.ProviderCommitments.Api.FakeServers;

public class CommitmentsOuterApiBuilder
{
    private readonly WireMockServer _server;

    private CommitmentsOuterApiBuilder(int port)
    {
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = port,
            UseSSL = true,
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
        });
    }

    public static CommitmentsOuterApiBuilder Create(int port) => new(port);

    public MockApi Build()
    {
        Console.WriteLine($"Approvals Outer Fake Api Running ({_server.Urls[0]})");
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
                    ApprenticeshipFunding = Array.Empty<object>()
                }));

        return this;
    }

    internal CommitmentsOuterApiBuilder WithIlrData()
    {
        _server.Given(Request.Create()
                .WithPath("/providers/*/learners/*")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new
                {
                    ULN = 5343273973,
                    UKPRN = 100123,
                    Firstname = "Dave",
                    Lastname = "Smith",
                    Email = "daves@email.test",
                    Dob = DateTime.Today.AddYears(-19),
                    AcademicYear = 2425,
                    StartDate = DateTime.Today.AddDays(50),
                    PlannedEndDate = DateTime.Today.AddDays(150),
                    PercentageLearningToBeDelivered = 50,
                    EpaoPrice = 1000,
                    TrainingPrice = 600,
                    AgreementId = 12345,
                    StandardCode = "AAAA",
                    IsflexiJob = true,
                    PlannedOTJTrainingHours = 60,
                }));

        _server.Given(Request.Create()
                .WithPath("/providers/*/learners")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new
                {
                    Page = 1,
                    TotalItems = 10,
                    PageSize = 5,
                    TotalPages = 2,
                    Items = new List<long> { 5343273973, 4663327397, 2743273973, 3488743973, 8264848936 }
                })
            );

        return this;
    }
}