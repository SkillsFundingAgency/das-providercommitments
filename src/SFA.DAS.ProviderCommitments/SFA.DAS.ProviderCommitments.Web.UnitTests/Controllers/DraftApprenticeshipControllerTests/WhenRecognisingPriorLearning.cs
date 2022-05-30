using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;
using SFA.DAS.Testing.AutoFixture;
using System;
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
        public async Task When_declaring_RPL_then_it_is_saved(bool? priorLearning)
        {
            var fixture = new WhenRecognisingPriorLearningFixture().ChoosePriorLearning(priorLearning);

            var result = await fixture.Sut.RecognisePriorLearning(fixture.ViewModel);

            fixture.ApiClient.Verify(x =>
                x.RecognisePriorLearning(
                    fixture.ViewModel.CohortId,
                    fixture.ViewModel.DraftApprenticeshipId,
                    It.Is<CommitmentsV2.Api.Types.Requests.RecognisePriorLearningRequest>(r =>
                        r.RecognisePriorLearning == priorLearning),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task After_declaring_there_is_prior_learning_then_show_RPL_details()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().ChoosePriorLearning(true);

            var result = await fixture.Sut.RecognisePriorLearning(fixture.ViewModel);

            result.VerifyRedirectsToRecognisePriorLearningDetailsPage(
                fixture.ViewModel.DraftApprenticeshipHashedId);
        }

        [Test]
        public async Task After_declaring_there_is_no_prior_learning_then_show_Cohort()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithoutStandardOptions()
                .ChoosePriorLearning(false);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            result.VerifyRedirectsToCohortDetailsPage(
                fixture.DetailsViewModel.ProviderId,
                fixture.DetailsViewModel.CohortReference);
        }

        [Test]
        public async Task After_declaring_there_is_no_prior_learning_then_show_Options_when_appropriate()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithStandardOptions()
                .ChoosePriorLearning(false);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            result.VerifyRedirectsToSelectOptionsPage(fixture.DetailsViewModel.DraftApprenticeshipHashedId);
        }

        [TestCase(1, 1)]
        [TestCase(2, null)]
        [TestCase(null, 3)]
        [TestCase(null, null)]
        public async Task When_previously_entered_details_then_show_them(int durationReducedBy, int priceReducedBy)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithPreviousDetails(durationReducedBy, priceReducedBy);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.Request);

            var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDetailsViewModel>();
            model.ReducedDuration.Should().Be(durationReducedBy);
            model.ReducedPrice.Should().Be(priceReducedBy);
        }

        [Test, MoqAutoData]
        public async Task When_submitting_RPL_details_then_it_is_saved(int reducedDuration, int reducedPrice)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .EnterRplDetails(reducedDuration, reducedPrice);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            fixture.ApiClient.Verify(x =>
                x.PriorLearningDetails(
                    fixture.DetailsViewModel.CohortId,
                    fixture.DetailsViewModel.DraftApprenticeshipId,
                    It.Is<CommitmentsV2.Api.Types.Requests.PriorLearningDetailsRequest>(r =>
                        r.DurationReducedBy == reducedDuration &&
                        r.PriceReducedBy == reducedPrice),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task After_submitting_prior_learning_details_then_show_Cohort()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithoutStandardOptions();

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            result.VerifyRedirectsToCohortDetailsPage(
                fixture.DetailsViewModel.ProviderId,
                fixture.DetailsViewModel.CohortReference);
        }

        [Test]
        public async Task After_submitting_prior_learning_details_then_show_Options_when_appropriate()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithStandardOptions();

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            result.VerifyRedirectsToSelectOptionsPage(fixture.DetailsViewModel.DraftApprenticeshipHashedId);
        }
    }

    public class WhenRecognisingPriorLearningFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        private readonly GetDraftApprenticeshipResponse Apprenticeship;
        public RecognisePriorLearningRequest Request;
        public RecognisePriorLearningViewModel ViewModel;
        public PriorLearningDetailsViewModel DetailsViewModel;
        public Mock<ICommitmentsApiClient> ApiClient { get; }

        public WhenRecognisingPriorLearningFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<RecognisePriorLearningRequest>();
            ViewModel = fixture.Create<RecognisePriorLearningViewModel>();
            ViewModel.IsTherePriorLearning = true;
            DetailsViewModel = fixture.Build<PriorLearningDetailsViewModel>().Create();
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
                    new RecognisePriorLearningViewModelToResultMapper(ApiClient.Object),
                    new RecognisePriorLearningRequestToDetailsViewModelMapper(ApiClient.Object),
                    new PriorLearningDetailsViewModelToResultMapper(ApiClient.Object)),
                Mock.Of<IEncodingService>(),
                Mock.Of<IAuthorizationService>(),
                new RecognitionOfPriorLearningConfiguration())
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

        internal WhenRecognisingPriorLearningFixture WithPreviousDetails(int durationReducedBy, int priceReducedBy)
        {
            Apprenticeship.DurationReducedBy = durationReducedBy;
            Apprenticeship.PriceReducedBy = priceReducedBy;
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

        internal WhenRecognisingPriorLearningFixture EnterRplDetails(int reducedDuration, int reducedPrice)
        {
            DetailsViewModel.ReducedDuration = reducedDuration;
            DetailsViewModel.ReducedPrice = reducedPrice;
            return this;
        }
    }
}