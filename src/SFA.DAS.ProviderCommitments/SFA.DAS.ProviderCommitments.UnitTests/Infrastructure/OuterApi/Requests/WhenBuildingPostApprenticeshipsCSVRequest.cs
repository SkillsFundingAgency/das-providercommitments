using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.Requests
{
    public class WhenBuildingPostApprenticeshipsCSVRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(long providerId, PostApprenticeshipsCSVRequest.Body data)
        {
            var actual = new PostApprenticeshipsCSVRequest(providerId, data);

            actual.PostUrl.Should().Be($"provider/{providerId}/apprenticeships/download");

            actual.Data.Should().BeEquivalentTo(data);
        }
    }
}
