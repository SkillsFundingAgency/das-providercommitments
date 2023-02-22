using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Features;

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

        [TestCase(1, 1, null, null, null, null)]
        [TestCase(2, null, null, null, null, null)]
        [TestCase(null, 3, null, null, null, null)]
        [TestCase(null, null, 10, 20, "1 ALevel", "Because of his qual")]
        [TestCase(null, null, 30, 2, null, "Because I like him/her")]
        public async Task When_previously_entered_details_then_map_them(int? durationReducedBy, int? priceReducedBy, int? durationReducedByHours, 
            int? weightageReduction, string qualifications, string reason)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithPreviousDetails(durationReducedBy, priceReducedBy, durationReducedByHours, weightageReduction, qualifications, reason);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.Request);

            var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDetailsViewModel>();
            model.ReducedDuration.Should().Be(durationReducedBy);
            model.ReducedPrice.Should().Be(priceReducedBy);
            model.DurationReducedByHours.Should().Be(durationReducedByHours);
            model.WeightageReducedBy.Should().Be(weightageReduction);
            model.QualificationsForRplReduction.Should().Be(qualifications);
            model.ReasonForRplReduction.Should().Be(reason);
        }

        [Test, MoqAutoData]
        public async Task When_submitting_RPL_details_then_it_is_saved(PriorLearningDetailsViewModel model)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .EnterRplDetails(model);

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            fixture.ApiClient.Verify(x =>
                x.PriorLearningDetails(
                    fixture.DetailsViewModel.CohortId,
                    fixture.DetailsViewModel.DraftApprenticeshipId,
                    It.Is<CommitmentsV2.Api.Types.Requests.PriorLearningDetailsRequest>(r =>
                        r.DurationReducedBy == model.ReducedDuration &&
                        r.PriceReducedBy == model.ReducedPrice &&
                        r.DurationReducedByHours == model.DurationReducedByHours &&
                        r.WeightageReducedBy == model.WeightageReducedBy &&
                        r.QualificationsForRplReduction == model.QualificationsForRplReduction &&
                        r.ReasonForRplReduction == model.ReasonForRplReduction &&
                        r.Rpl2Mode == false
                    ),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task When_submitting_RPL_details_then_rpl2mode_is_set()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithRpl2Mode();

            var result = await fixture.Sut.RecognisePriorLearningDetails(fixture.DetailsViewModel);

            fixture.ApiClient.Verify(x =>
                x.PriorLearningDetails(
                    fixture.DetailsViewModel.CohortId,
                    fixture.DetailsViewModel.DraftApprenticeshipId,
                    It.Is<CommitmentsV2.Api.Types.Requests.PriorLearningDetailsRequest>(r =>
                        r.Rpl2Mode == true
                    ),
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
        public Mock<IAuthorizationService> AuthorizationService { get; }

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

            AuthorizationService = new Mock<IAuthorizationService>();

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                ApiClient.Object,
                new SimpleModelMapper(
                    new RecognisePriorLearningRequestToViewModelMapper(ApiClient.Object),
                    new RecognisePriorLearningViewModelToResultMapper(ApiClient.Object),
                    new RecognisePriorLearningRequestToDetailsViewModelMapper(ApiClient.Object),
                    new PriorLearningDetailsViewModelToResultMapper(ApiClient.Object, AuthorizationService.Object)),
                Mock.Of<IEncodingService>(),
                Mock.Of<IAuthorizationService>(),
                Mock.Of<IOuterApiService>())
            {
                TempData = Mock.Of<ITempDataDictionary>()
            };
        }

        internal WhenRecognisingPriorLearningFixture WithRpl2Mode()
        {
            AuthorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.Rpl2)).ReturnsAsync(true);
            return this;
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

        internal WhenRecognisingPriorLearningFixture WithPreviousDetails(int? durationReducedBy, int? priceReducedBy, int? durationReducedByHours, 
            int? weightageReducedBy, string qualificationsForRplReduction, string reasonForRplReduction)
        {
            Apprenticeship.DurationReducedBy = durationReducedBy;
            Apprenticeship.PriceReducedBy = priceReducedBy;
            Apprenticeship.DurationReducedByHours = durationReducedByHours;
            Apprenticeship.WeightageReducedBy = weightageReducedBy;
            Apprenticeship.QualificationsForRplReduction = qualificationsForRplReduction;
            Apprenticeship.ReasonForRplReduction = reasonForRplReduction;
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

        internal WhenRecognisingPriorLearningFixture EnterRplDetails(PriorLearningDetailsViewModel model)
        {
            DetailsViewModel.ReducedDuration = model.ReducedDuration;
            DetailsViewModel.ReducedPrice = model.ReducedPrice;
            DetailsViewModel.DurationReducedByHours = model.DurationReducedByHours;
            DetailsViewModel.WeightageReducedBy = model.WeightageReducedBy;
            DetailsViewModel.QualificationsForRplReduction = model.QualificationsForRplReduction;
            DetailsViewModel.ReasonForRplReduction = model.ReasonForRplReduction;
            return this;
        }

    }
}