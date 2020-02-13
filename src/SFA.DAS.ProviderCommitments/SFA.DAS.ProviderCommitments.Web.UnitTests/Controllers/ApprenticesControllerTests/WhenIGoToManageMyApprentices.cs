using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenIGoToManageMyApprentices
    {
        [Test, MoqAutoData]
        public async Task ThenTheMappedViewModelIsReturned(
            long providerId,
            ApprenticesFilterModel filterModel,
            IndexRequest request,
            IndexViewModel expectedViewModel,
            [Frozen] Mock<IModelMapper> apprenticeshipMapper,
            ApprenticeController controller)
        {
            //Arrange
            apprenticeshipMapper
                .Setup(mapper => mapper.Map<IndexViewModel>(request))
                .ReturnsAsync(expectedViewModel);

            //Act
            var result = await controller.Index(request) as ViewResult;
            var actualModel = result.Model as IndexViewModel;

            //Assert
            actualModel.Should().BeEquivalentTo(expectedViewModel);
        }

        [Test, MoqAutoData]
        public async Task Then_SortedByHeaderClassName_Set(
            IndexRequest request,
            IndexViewModel expectedViewModel,
            [Frozen] Mock<IModelMapper> apprenticeshipMapper,
            ApprenticeController controller)
        {
            //Arrange
            expectedViewModel.FilterModel.ReverseSort = false;
            apprenticeshipMapper
                .Setup(mapper => mapper.Map<IndexViewModel>(request))
                .ReturnsAsync(expectedViewModel);

            //Act
            var result = await controller.Index(request) as ViewResult;
            var actualModel = result.Model as IndexViewModel;

            //Assert
            actualModel.SortedByHeaderClassName.Should().EndWith("das-table__sort--asc");
        }
    }
}