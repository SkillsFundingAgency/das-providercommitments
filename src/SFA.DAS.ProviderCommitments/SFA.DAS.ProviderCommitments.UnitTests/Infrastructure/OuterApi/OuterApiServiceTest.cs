using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi
{
    public class OuterApiServiceTest
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Made_And_ProviderResponse_Returned(
            long ukprn,
            ProviderAccountResponse apiResponse,
            [Frozen] Mock<IOuterApiClient> apiClient,
            OuterApiService service)
        {
            //Arrange
            var request = new GetProviderStatusDetails(ukprn);
            apiClient.Setup(x =>
                    x.Get<ProviderAccountResponse>(
                        It.Is<GetProviderStatusDetails>(c => c.GetUrl.Equals(request.GetUrl))))
                .ReturnsAsync(apiResponse);

            //Act
            var actual = await service.GetProviderStatus(ukprn);

            //Assert
            actual.CanAccessService.Should().Be(apiResponse.CanAccessService);
        }
    }
}
