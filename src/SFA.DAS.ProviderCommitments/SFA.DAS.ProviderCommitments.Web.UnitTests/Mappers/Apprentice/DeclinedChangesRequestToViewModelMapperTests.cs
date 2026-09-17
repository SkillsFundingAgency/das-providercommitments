using System;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class DeclinedChangesRequestToViewModelMapperTests
{
    [Test, MoqAutoData]
    public async Task Map_ThenCollapsesPriceFieldsAndAppliesDeclinedCopy(
        InvalidIlrChangesRequest request,
        GetInvalidIlrChangesResponse response,
        [Frozen] Mock<IOuterApiClient> outerApiClient,
        DeclinedChangesRequestToViewModelMapper mapper)
    {
        response.FirstName = "Barry";
        response.LastName = "Led";
        response.RequestSets =
        [
            new InvalidIlrChangeSet
            {
                ApprovalRequestId = Guid.NewGuid(),
                Decision = "Declined",
                Fields =
                [
                    new InvalidIlrChangeField { Field = "TNP1", Old = "7268", New = "8268" },
                    new InvalidIlrChangeField { Field = "TNP2", Old = "0", New = "0" }
                ]
            }
        ];

        outerApiClient.Setup(x => x.Get<GetInvalidIlrChangesResponse>(
                It.Is<GetInvalidIlrChangesRequest>(apiRequest =>
                    apiRequest.GetUrl == $"provider/{request.ProviderId}/apprentices/{request.ApprenticeshipId}/declined-changes")))
            .ReturnsAsync(response);

        var result = await mapper.Map(request);

        result.Should().BeOfType<DeclinedChangesViewModel>();
        result.LearnerName.Should().Be("Barry Led");
        result.Heading.Should().Be("Changes declined for Barry Led");
        result.Intro.Should().Be("These changes were declined by your employer.");
        result.NewValueColumnHeader.Should().Be("Declined");
        result.FieldColumnHeader.Should().BeEmpty();
        result.LegendHint.Should().Be("You will still see the changes in your change history but the alert will disappear.");
        result.RequestSets[0].Fields.Should().ContainSingle();
        result.RequestSets[0].Fields[0].FieldDisplayName.Should().Be("Total price");
        result.RequestSets[0].Fields[0].Old.Should().Be("£7,268");
        result.RequestSets[0].Fields[0].New.Should().Be("£8,268");
    }
}
