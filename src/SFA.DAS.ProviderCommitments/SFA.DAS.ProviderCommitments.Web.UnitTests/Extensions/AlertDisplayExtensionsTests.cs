using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

public class AlertDisplayExtensionsTests
{
    [Test]
    public void ToAlertDisplayText_ThenUsesIlrChangeInvalidCopy()
    {
        AlertDisplayExtensions.IlrChangeInvalid.ToAlertDisplayText().Should().Be("ILR change invalid");
        ((Alerts)5).ToAlertDisplayText().Should().Be("ILR change invalid");
    }

    [Test]
    public void IsIlrChangeInvalid_ThenMatchesTheDisplayText()
    {
        "ILR change invalid".IsIlrChangeInvalid().Should().BeTrue();
        "Changes pending".IsIlrChangeInvalid().Should().BeFalse();
    }
}
