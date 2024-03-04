using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Requests.Provider;

public class WhenBuildingGetProviderAccountRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Constructed(int ukprn)
    {
        var actual = new GetProviderStatusDetails(ukprn);

        actual.GetUrl.Should().Be($"provideraccounts/{ukprn}");
    }
}