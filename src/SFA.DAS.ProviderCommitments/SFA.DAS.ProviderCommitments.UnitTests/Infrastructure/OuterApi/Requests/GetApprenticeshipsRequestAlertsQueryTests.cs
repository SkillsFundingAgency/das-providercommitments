using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Requests;

public class GetApprenticeshipsRequestAlertsQueryTests
{
    [Test]
    public void GetUrl_ThenSendsIlrChangeInvalidByName()
    {
        var request = new GetApprenticeshipsRequest(
            100,
            1,
            25,
            null,
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            Alerts.IlrChangeInvalid,
            null,
            null);

        request.GetUrl.Should().Contain("alert=IlrChangeInvalid");
        request.GetUrl.Should().NotContain("ChangesPending");
        request.GetUrl.Should().NotContain("ConfirmDates");
    }
}
