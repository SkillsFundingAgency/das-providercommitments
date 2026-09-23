using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Requests;

public class GetApprenticeshipsRequestAlertsQueryTests
{
    [TestCase(Alerts.IlrChangeInvalid)]
    [TestCase(Alerts.ChangesDeclined)]
    public void GetUrl_ThenAlertQueryValueIsTheEnumName(Alerts alert)
    {
        var request = new GetApprenticeshipsRequest(
            10001234,
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
            alert,
            null,
            null);

        request.GetUrl.Should().Contain($"alert={alert}");
        request.GetUrl.Should().NotContain(",");
        request.GetUrl.Should().NotContain("ChangesPending");
        request.GetUrl.Should().NotContain("ConfirmDates");
    }
}
