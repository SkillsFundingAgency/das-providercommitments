﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Authorization;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.ProviderPermissions;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi;

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

    [Test, MoqAutoData]
    public async Task Then_CanAccessApprenticeship_Request_Is_Made_And_Response_Returned(
        Party party,
        long partyId,
        long apprenticeshipId,
        GetApprenticeshipAccessResponse apiResponse,
        [Frozen] Mock<IOuterApiClient> apiClient,
        OuterApiService service)
    {
        //Arrange
        apiClient.Setup(x =>
                x.Get<GetApprenticeshipAccessResponse>(
                    It.Is<GetApprenticeshipAccessRequest>(c => c.Party.Equals(party) && c.PartyId.Equals(partyId) && c.ApprenticeshipId.Equals(apprenticeshipId))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.CanAccessApprenticeship(party, partyId, apprenticeshipId);

        //Assert
        actual.Should().Be(apiResponse.HasApprenticeshipAccess);
    }

    [Test, MoqAutoData]
    public async Task Then_CanAccessCohort_Request_Is_Made_And_Response_Returned(
        Party party,
        long partyId,
        long cohortId,
        GetCohortAccessResponse apiResponse,
        [Frozen] Mock<IOuterApiClient> apiClient,
        OuterApiService service)
    {
        //Arrange
        apiClient.Setup(x =>
                x.Get<GetCohortAccessResponse>(
                    It.Is<GetCohortAccessRequest>(c => c.Party.Equals(party) && c.PartyId.Equals(partyId) && c.CohortId.Equals(cohortId))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.CanAccessCohort(party, partyId, cohortId);

        //Assert
        actual.Should().Be(apiResponse.HasCohortAccess);
    }

    [Test, MoqAutoData]
    public async Task Then_HasPermission_Request_Is_Made_And_Response_Returned(
        long ukprn,
        long accountLegalEntityId,
        Operation operation,
        GetHasPermissionResponse apiResponse,
        [Frozen] Mock<IOuterApiClient> apiClient,
        OuterApiService service)
    {
        //Arrange
        var request = new GetHasPermissionRequest(ukprn, accountLegalEntityId, operation);
        apiClient.Setup(x =>
                x.Get<GetHasPermissionResponse>(
                    It.Is<GetHasPermissionRequest>(c => c.GetUrl.Equals(request.GetUrl))))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await service.HasPermission(ukprn, accountLegalEntityId, operation);

        //Assert
        actual.Should().Be(apiResponse.HasPermission);
    }
}