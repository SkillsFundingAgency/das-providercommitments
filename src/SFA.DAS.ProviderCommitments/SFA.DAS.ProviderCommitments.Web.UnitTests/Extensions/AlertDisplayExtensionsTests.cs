using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

public class AlertDisplayExtensionsTests
{
    [Test]
    public void ToAlertDisplayText_ThenUsesIlrChangeInvalidCopy()
    {
        Alerts.IlrChangeInvalid.ToAlertDisplayText().Should().Be("ILR change invalid");
    }

    [Test]
    public void IsIlrChangeInvalid_ThenMatchesTheDisplayText()
    {
        "ILR change invalid".IsIlrChangeInvalid().Should().BeTrue();
        "Changes pending".IsIlrChangeInvalid().Should().BeFalse();
    }

    [Test]
    public void ToAlertDisplayText_ThenUsesChangesDeclinedCopy()
    {
        Alerts.ChangesDeclined.ToAlertDisplayText().Should().Be("Changes declined");
    }

    [Test]
    public void IsChangesDeclined_ThenMatchesTheDisplayText()
    {
        "Changes declined".IsChangesDeclined().Should().BeTrue();
        "ILR change invalid".IsChangesDeclined().Should().BeFalse();
    }
}
