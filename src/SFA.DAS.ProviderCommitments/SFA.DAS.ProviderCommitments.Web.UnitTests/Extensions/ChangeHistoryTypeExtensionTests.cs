using FluentAssertions;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions;

[TestFixture]
public class ChangeHistoryTypeExtensionTests
{
    [Test]
    public void GetDisplayClass_WhenAutoRejected_ReturnsRedTag()
    {
        LearningChangeType.AutoRejected.GetDisplayClass().Should().Be("govuk-tag--red");
    }

    [Test]
    public void GetEnumDescription_WhenAutoRejected_ReturnsAutoRejectedLabel()
    {
        LearningChangeType.AutoRejected.GetEnumDescription().Should().Be("Auto-rejected");
    }
}
