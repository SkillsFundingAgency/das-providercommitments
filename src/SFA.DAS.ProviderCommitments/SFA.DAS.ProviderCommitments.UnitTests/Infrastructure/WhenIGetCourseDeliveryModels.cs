using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure;

public class WhenIGetCourseDeliveryModels
{
    [Test, MoqAutoData]
    public async Task Then_delivery_models_are_returned(
        [Frozen] IApprovalsOuterApiHttpClient htpClient,
        ApprovalsOuterApiClient sut,
        ProviderCourseDeliveryModels models,
        long provider,
        string course,
        long accountLegalEntityId)
    {
        Mock.Get(htpClient).Setup(x => x
                .Get<ProviderCourseDeliveryModels>(
                    $"providers/{provider}/courses?trainingCode={course}&accountLegalEntityId={accountLegalEntityId}", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await sut.GetProviderCourseDeliveryModels(provider, course, accountLegalEntityId);

        result.Should().BeEquivalentTo(new ProviderCourseDeliveryModels()
        {
            DeliveryModels = models.DeliveryModels
        });
    }
}