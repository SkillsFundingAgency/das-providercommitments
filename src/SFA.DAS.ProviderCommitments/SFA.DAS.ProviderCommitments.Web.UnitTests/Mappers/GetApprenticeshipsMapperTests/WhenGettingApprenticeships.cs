using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.GetApprenticeshipsMapperTests
{
    public class WhenGettingApprenticeships
    {
        [Test, MoqAutoData]
        public async Task ShouldMapValues(
            GetApprenticeshipsRequest request,
            [Frozen]GetApprenticeshipsResponse clientResponse,
            Mock<ICommitmentsApiClient> client,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            client.Setup(x => x.GetApprenticeships(request.ProviderId, request.PageNumber, 
                    request.PageItemCount, null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            //Act
            var viewModel = await mapper.Map(request);

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(request.ProviderId, viewModel.ProviderId);
            Assert.AreEqual(clientResponse.Apprenticeships, viewModel.Apprenticeships);
            Assert.IsNotNull(viewModel.FilterModel);
            Assert.AreEqual(clientResponse.TotalApprenticeshipsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsFound);
            Assert.AreEqual(clientResponse.TotalApprenticeshipsWithAlertsFound, viewModel.FilterModel.TotalNumberOfApprenticeshipsWithAlertsFound);
            Assert.AreEqual(request.PageNumber, viewModel.FilterModel.PageNumber);
        }

        [Test]
        [MoqInlineAutoData(0, false)]
        [MoqInlineAutoData(1, true)]
        [MoqInlineAutoData(2, true)]
        public async Task ThenAnyApprenticeshipsIsSetWhenApprenticeshipsIsNotNull(
            int numberOfApprenticeships, 
            bool expected,
            ApprenticeshipDetails approvedApprenticeship,
            GetApprenticeshipsRequest request,
            [Frozen]GetApprenticeshipsResponse clientResponse,
            Mock<ICommitmentsApiClient> client,
            GetApprenticeshipsRequestMapper mapper)
        {
            //Arrange
            var apprenticeships = new List<ApprenticeshipDetails>();

            for (var i = 0; i < numberOfApprenticeships; i++)
            {
                apprenticeships.Add(approvedApprenticeship);
            }

            clientResponse.Apprenticeships = apprenticeships;
           
            client.Setup(x => x.GetApprenticeships(request.ProviderId, request.PageNumber, 
                    request.PageItemCount, null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientResponse);

            //Act
            var viewModel = await mapper.Map(request);

            //Assert
            Assert.AreEqual(viewModel.AnyApprenticeships, expected);
        }
    }
}
