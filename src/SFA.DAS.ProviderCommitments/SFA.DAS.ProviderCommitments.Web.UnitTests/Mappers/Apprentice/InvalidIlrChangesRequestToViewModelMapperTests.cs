using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class InvalidIlrChangesRequestToViewModelMapperTests
{
    [Test, MoqAutoData]
    public async Task Map_ThenBuildsThePageModelFromTheOuterApi(
        InvalidIlrChangesRequest request,
        GetInvalidIlrChangesResponse response,
        [Frozen] Mock<IOuterApiClient> outerApiClient,
        InvalidIlrChangesRequestToViewModelMapper mapper)
    {
        response.FirstName = "Jane";
        response.LastName = "Doe";
        response.RequestSets =
        [
            new InvalidIlrChangeSet
            {
                ApprovalRequestId = Guid.NewGuid(),
                Decision = "Auto rejected",
                Fields =
                [
                    new InvalidIlrChangeField
                    {
                        Field = "TNP1",
                        Old = "1000",
                        New = "0",
                        EffectiveFrom = new DateTime(2026, 8, 1),
                        Reason = "Price is zero"
                    },
                    new InvalidIlrChangeField
                    {
                        Field = "TNP2",
                        Old = "500",
                        New = "600",
                        EffectiveFrom = new DateTime(2026, 8, 1),
                        Reason = "Over cap"
                    }
                ]
            }
        ];

        outerApiClient.Setup(x => x.Get<GetInvalidIlrChangesResponse>(
                It.Is<GetInvalidIlrChangesRequest>(apiRequest =>
                    apiRequest.GetUrl == $"{request.ProviderId}/apprentices/{request.ApprenticeshipId}/invalid-ilr-changes")))
            .ReturnsAsync(response);

        var result = await mapper.Map(request);

        result.ProviderId.Should().Be(request.ProviderId);
        result.ApprenticeshipHashedId.Should().Be(request.ApprenticeshipHashedId);
        result.ApprenticeshipId.Should().Be(request.ApprenticeshipId);
        result.LearnerName.Should().Be("Jane Doe");
        result.RequestSets.Should().HaveCount(1);
        result.RequestSets[0].Decision.Should().Be("Auto rejected");
        result.RequestSets[0].Fields[0].FieldDisplayName.Should().Be("Training price");
        result.RequestSets[0].Fields[1].FieldDisplayName.Should().Be("End-point assessment price");
        result.RequestSets[0].Fields[0].Old.Should().Be("1000");
        result.RequestSets[0].Fields[0].New.Should().Be("0");
    }
}
