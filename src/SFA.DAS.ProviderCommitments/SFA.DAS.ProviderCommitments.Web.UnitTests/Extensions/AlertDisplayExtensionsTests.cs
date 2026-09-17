using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

public class AlertDisplayExtensionsTests
{
    [Test]
    public void ToAlertDisplayText_ThenUsesIlrChangeInvalidCopy()
    {
        Alerts.IlrChangeInvalid.ToAlertDisplayText().Should().Be("ILR changes invalid");
    }

    [Test]
    public void IsIlrChangeInvalid_ThenMatchesTheDisplayText()
    {
        "ILR changes invalid".IsIlrChangeInvalid().Should().BeTrue();
        "Changes pending".IsIlrChangeInvalid().Should().BeFalse();
    }
}
