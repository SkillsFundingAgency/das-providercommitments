﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetCourseDeliveryModels
{
    public class WhenIGetCourseDeliveryModels
    {
        [Test, MoqAutoData]
        public async Task Then_delivery_models_are_returned(
            [Frozen] IApprovalsOuterApiHttpClient client,
            GetProviderCourseDeliveryModelsQueryHandler handler,
            ProviderCourseDeliveryModels models,
            long provider,
            string course)
        {
            Mock.Get(client).Setup(x => x
                .Get<ProviderCourseDeliveryModels>(
                    $"/providers/{provider}/courses/{course}", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(models);

            var result = await handler.Handle(new GetProviderCourseDeliveryModelsQueryRequest
            {
                ProviderId = provider,
                CourseId = course,
            }, CancellationToken.None);

            result.Should().BeEquivalentTo(new GetProviderCourseDeliveryModelsQueryResponse
            {
                DeliveryModels = models.DeliveryModels
            });
        }
    }
}