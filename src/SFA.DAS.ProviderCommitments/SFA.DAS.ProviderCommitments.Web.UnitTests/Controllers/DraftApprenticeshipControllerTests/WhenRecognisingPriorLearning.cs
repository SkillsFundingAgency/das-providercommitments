using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    public class WhenRecognisingPriorLearning
    {
        [Test]
        public async Task Get_returns_view()
        {
            var fixture = new WhenRecognisingPriorLearningFixture();

            var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

            result.VerifyReturnsViewModel().ViewName.Should().Be("RecognisePriorLearning");
        }

        [Test]
        public async Task When_not_previously_selected_then_show_no_selection()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithoutPreviousSelection();

            var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

            result.VerifyReturnsViewModel().WithModel<RecognisePriorLearningViewModel>()
                .IsTherePriorLearning.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task When_previously_selected_then_show_selection(bool previousSelection)
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithPreviousSelection(previousSelection);

            var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

            result.VerifyReturnsViewModel().WithModel<RecognisePriorLearningViewModel>()
                .IsTherePriorLearning.Should().Be(previousSelection);
        }

        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public async Task When_submitting_selection_then_it_is_saved(bool? priorLearning)
        {
            var fixture = new WhenRecognisingPriorLearningFixture().ChoosePriorLearning(priorLearning);

            var result = await fixture.Sut.RecognisePriorLearning(fixture.ViewModel);

            fixture.ApiClient.Verify(x =>
                x.UpdateDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), null, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task After_submitting_prior_learning_then_show_Cohort()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithoutStandardOptions();

            var result = await fixture.Sut.RecognisePriorLearning(fixture.ViewModel);

            result.VerifyRedirectsToCohortDetailsPage(
                fixture.ViewModel.ProviderId,
                fixture.ViewModel.CohortReference);
        }

        [Test]
        public async Task After_submitting_prior_learning_then_show_Options_when_appropriate()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithStandardOptions();

            var result = await fixture.Sut.RecognisePriorLearning(fixture.ViewModel);

            result.VerifyRedirectsToSelectOptionsPage(fixture.ViewModel.DraftApprenticeshipHashedId);
        }
    }

    public class WhenRecognisingPriorLearningFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        private readonly GetDraftApprenticeshipResponse Apprenticeship;
        public RecognisePriorLearningRequest Request;
        public RecognisePriorLearningViewModel ViewModel;
        public Mock<ICommitmentsApiClient> ApiClient { get; }

        public WhenRecognisingPriorLearningFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<RecognisePriorLearningRequest>();
            ViewModel = fixture.Create<RecognisePriorLearningViewModel>();
            ViewModel.IsTherePriorLearning = true;
            Apprenticeship = fixture.Create<GetDraftApprenticeshipResponse>();

            ApiClient = new Mock<ICommitmentsApiClient>();
            ApiClient.Setup(x =>
                x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Apprenticeship);

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                ApiClient.Object,
                new SimpleModelMapper(
                    new RecognisePriorLearningRequestToViewModelMapper(ApiClient.Object),
                    new RecognisePriorLearningViewModelToResultMapper(ApiClient.Object)),
                Mock.Of<IEncodingService>(),
                Mock.Of<IAuthorizationService>())
            {
                TempData = Mock.Of<ITempDataDictionary>()
            };
        }

        internal WhenRecognisingPriorLearningFixture WithoutPreviousSelection()
        {
            Apprenticeship.RecognisePriorLearning = null;
            return this;
        }

        internal WhenRecognisingPriorLearningFixture WithPreviousSelection(bool previousSelection)
        {
            Apprenticeship.RecognisePriorLearning = previousSelection;
            return this;
        }

        internal WhenRecognisingPriorLearningFixture ChoosePriorLearning(bool? priorLearning)
        {
            ViewModel.IsTherePriorLearning = priorLearning;
            return this;
        }

        internal WhenRecognisingPriorLearningFixture WithoutStandardOptions()
        {
            Apprenticeship.HasStandardOptions = false;
            return this;
        }

        internal WhenRecognisingPriorLearningFixture WithStandardOptions()
        {
            Apprenticeship.HasStandardOptions = true;
            return this;
        }
    }
}