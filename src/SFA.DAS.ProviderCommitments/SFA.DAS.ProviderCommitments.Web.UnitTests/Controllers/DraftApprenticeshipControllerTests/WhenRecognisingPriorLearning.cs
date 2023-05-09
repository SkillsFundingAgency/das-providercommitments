using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices.ChangeEmployer;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;
using SFA.DAS.Testing.AutoFixture;
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

        [TestCase(100, 1, null, null)]
        [TestCase(2, null, null, null)]
        [TestCase(null, 3, null, null)]
        [TestCase(null, null, null, null)]
        [TestCase(null, null, 100, 10)]
        public async Task When_previously_rpl_data_exist_then_map_them(int? totalHours, int? hoursReducedBy, int? costBeforeRpl, int? priceReducedBy)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithRplSummary(false)
                .WithPreviousRplData(totalHours, hoursReducedBy, null, null, costBeforeRpl, priceReducedBy);

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.Request);

            var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDataViewModel>();
            model.TrainingTotalHours.Should().Be(totalHours);
            model.DurationReducedByHours.Should().Be(hoursReducedBy);
            model.CostBeforeRpl.Should().Be(costBeforeRpl);
            model.PriceReduced.Should().Be(priceReducedBy);
        }



        [Test]
        public async Task When_accessing_RecognisePriorLearningData_if_is_not_in_rpl_enhanced_mode()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithRplSummary(false);

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.Request);

            var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDataViewModel>();
        }



        [Test, MoqAutoData]
        public async Task When_submitting_RPL_data_then_it_is_saved(PriorLearningDataViewModel model)
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .EnterRplData(model);

            await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            fixture.OuterApiService.Verify(x =>
                x.UpdatePriorLearningData(
                    fixture.DataViewModel.ProviderId,
                    fixture.DataViewModel.CohortId,
                    fixture.DataViewModel.DraftApprenticeshipId,
                    It.Is<CreatePriorLearningDataApimRequest>(r =>
                        r.TrainingTotalHours == model.TrainingTotalHours &&
                        r.DurationReducedByHours == model.DurationReducedByHours &&
                        r.IsDurationReducedByRpl == model.IsDurationReducedByRpl &&
                        r.DurationReducedBy == model.DurationReducedBy &&
                        r.CostBeforeRpl == model.CostBeforeRpl &&
                        r.PriceReducedBy == model.PriceReduced
                    )));
        }

        [Test]
        public async Task When_submitting_RPL_data_which_hold_a_value_inside_the_IsDurationReducedByRpl_but_that_field_is_set_to_No()
        {
            var model = new PriorLearningDataViewModel
            {
                IsDurationReducedByRpl = false,
                DurationReducedBy = 10
            };

            var fixture = new WhenRecognisingPriorLearningFixture()
                .EnterRplData(model)
                .WithRplDataResult(true, true);

            await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            fixture.OuterApiService.Verify(x =>
                x.UpdatePriorLearningData(
                    fixture.DataViewModel.ProviderId,
                    fixture.DataViewModel.CohortId,
                    fixture.DataViewModel.DraftApprenticeshipId,
                    It.Is<CreatePriorLearningDataApimRequest>(r =>
                        r.IsDurationReducedByRpl == false &&
                        r.DurationReducedBy == 10
                    )));
        }


        [TestCase, MoqAutoData]
        public async Task After_submitting_prior_learning_data_then_show_RPL_summary_page()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithoutStandardOptions()
                .WithRplSummary(true);             

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            result.VerifyRedirectsToRecognisePriorLearningSummaryPage(
                fixture.DataViewModel.DraftApprenticeshipHashedId);
        }

        [TestCase, MoqAutoData]
        public async Task After_submitting_prior_learning_data_and_no_standards_and_no_rpl_error_then_dont_show_RPL_summary_page()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithoutStandardOptions()
                .WithRplSummary(false)
                .WithRplDataResult(true, true);

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            result.VerifyRedirectsToCohortDetailsPage(
                fixture.DataViewModel.ProviderId,
                fixture.DataViewModel.CohortReference);
        }

        [TestCase, MoqAutoData]
        public async Task After_submitting_prior_learning_data_with_standards_and_no_rpl_error_then_dont_show_RPL_summary_page()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithStandardOptions()
                .WithRplSummary(false);

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            result.VerifyRedirectsToSelectOptionsPage(fixture.DataViewModel.DraftApprenticeshipHashedId);
        }




        [Test]
        public async Task After_submitting_prior_learning_data_then_show_Cohort()
        {
            var fixture = new WhenRecognisingPriorLearningFixture()
                .WithoutStandardOptions()
                .WithRplSummary(false);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            result.VerifyRedirectsToCohortDetailsPage(
                fixture.DataViewModel.ProviderId,
                fixture.DataViewModel.CohortReference);
        }

        [Test]
        public async Task After_submitting_prior_learning_data_then_show_Options_when_appropriate()
        {
            var fixture = new WhenRecognisingPriorLearningFixture().WithStandardOptions();

            var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

            result.VerifyRedirectsToSelectOptionsPage(fixture.DataViewModel.DraftApprenticeshipHashedId);
        }
    }

    public class WhenRecognisingPriorLearningFixture
    {
        public DraftApprenticeshipController Sut { get; set; }

        private readonly GetDraftApprenticeshipResponse Apprenticeship;
        private readonly GetPriorLearningSummaryQueryResult RplSummary;
        public RecognisePriorLearningRequest Request;
        public RecognisePriorLearningViewModel ViewModel;
        public PriorLearningDetailsViewModel DetailsViewModel;
        public PriorLearningDataViewModel DataViewModel;
        public CreatePriorLearningDataResponse RplCreatePriorLearningDataResponse;
        public GetPriorLearningDataQueryResult PriorLearningDataQueryResult;
        public CreatePriorLearningDataApimRequest CreatePriorLearningDataApimRequest;

        public Mock<IOuterApiService> OuterApiService;
        public Mock<IOuterApiClient> OuterApiClient;

        public GetApprenticeshipResponse ApprenticeshipResponse { get; set; }
        private GetEditApprenticeshipResponse _editApprenticeshipResponse;
        public EditApprenticeshipRequest _request;

        public RecognisePriorLearningResult RplDataResult;

        public Mock<ICommitmentsApiClient> ApiClient { get; }
        public Mock<IAuthorizationService> AuthorizationService { get; }

        public WhenRecognisingPriorLearningFixture()
        {
            var fixture = new Fixture();
            Request = fixture.Create<RecognisePriorLearningRequest>();
            ViewModel = fixture.Create<RecognisePriorLearningViewModel>();
            ViewModel.IsTherePriorLearning = true;
            DetailsViewModel = fixture.Build<PriorLearningDetailsViewModel>().Create();
            DataViewModel = fixture.Build<PriorLearningDataViewModel>().Create();
            Apprenticeship = fixture.Create<GetDraftApprenticeshipResponse>();
            RplSummary = fixture.Create<GetPriorLearningSummaryQueryResult>();
            RplDataResult = fixture.Create<RecognisePriorLearningResult>();
            PriorLearningDataQueryResult = fixture.Create<GetPriorLearningDataQueryResult>();
            RplCreatePriorLearningDataResponse = fixture.Create<CreatePriorLearningDataResponse>();
            CreatePriorLearningDataApimRequest = fixture.Create<CreatePriorLearningDataApimRequest>();

            ApiClient = new Mock<ICommitmentsApiClient>();
            ApiClient.Setup(x =>
                x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Apprenticeship);

            OuterApiService = new Mock<IOuterApiService>();
            OuterApiService.Setup(x => x.GetPriorLearningSummary(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(RplSummary);
            OuterApiService.Setup(x => x.UpdatePriorLearningData(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), CreatePriorLearningDataApimRequest)).ReturnsAsync(RplCreatePriorLearningDataResponse);
            OuterApiService.Setup(x => x.GetPriorLearningData(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(PriorLearningDataQueryResult);

            OuterApiClient = new Mock<IOuterApiClient>();
            OuterApiClient.Setup(x => x.Get<GetEditApprenticeshipResponse>(It.Is<GetEditApprenticeshipRequest>(r =>
                    r.ApprenticeshipId == _request.ApprenticeshipId && r.ProviderId == _request.ProviderId)))
                .ReturnsAsync(_editApprenticeshipResponse);

            AuthorizationService = new Mock<IAuthorizationService>();

            Sut = new DraftApprenticeshipController(
                Mock.Of<IMediator>(),
                ApiClient.Object,
                new SimpleModelMapper(
                    new RecognisePriorLearningRequestToViewModelMapper(ApiClient.Object),
                    new RecognisePriorLearningViewModelToResultMapper(ApiClient.Object, OuterApiClient.Object),
                    new RecognisePriorLearningRequestToDetailsViewModelMapper(ApiClient.Object),
                    new RecognisePriorLearningRequestToDataViewModelMapper(OuterApiService.Object),
                    new PriorLearningDetailsViewModelToResultMapper(ApiClient.Object, AuthorizationService.Object, OuterApiClient.Object),
                    new RecognisePriorLearningSummaryRequestToSummaryViewModelMapper(OuterApiService.Object),
                    new PriorLearningDataViewModelToResultMapper(OuterApiService.Object)),
                    
                Mock.Of<IEncodingService>(),
                    AuthorizationService.Object,
                OuterApiService.Object);
        }

        internal WhenRecognisingPriorLearningFixture WithRpl2Mode()
        {
            AuthorizationService.Setup(x => x.IsAuthorized(ProviderFeature.RplExtended)).Returns(true);
            AuthorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.RplExtended)).ReturnsAsync(true);
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

        internal WhenRecognisingPriorLearningFixture WithPreviousRplData(int? trainingTotalHours, int? durationReducedByHours, bool? isDurationReducedByRpl, int? reducedDuration,
            int? costBeforeRpl, int? reducedPrice)
        {
            Apprenticeship.TrainingTotalHours = trainingTotalHours;
            Apprenticeship.DurationReducedByHours = durationReducedByHours;
            Apprenticeship.IsDurationReducedByRpl = isDurationReducedByRpl;
            Apprenticeship.DurationReducedBy = reducedDuration;
            Apprenticeship.CostBeforeRpl = costBeforeRpl;
            Apprenticeship.PriceReducedBy = reducedPrice;
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

        internal WhenRecognisingPriorLearningFixture WithRplSummary(bool rplPriceReductionError)
        { 
            RplSummary.TrainingTotalHours = 100;
            RplSummary.DurationReducedByHours = 10;
            RplSummary.CostBeforeRpl = 10000;
            RplSummary.PriceReducedBy = 1000;
            RplSummary.FundingBandMaximum = 1000;
            RplSummary.PercentageOfPriorLearning = 10;
            RplSummary.MinimumPercentageReduction = 10;
            RplSummary.MinimumPriceReduction = 10;
            RplSummary.RplPriceReductionError = rplPriceReductionError;
            return this;
        }

        internal WhenRecognisingPriorLearningFixture WithRplDataResult(bool hasStandardOptions, bool rplPriceReductionError)
        {
            RplDataResult.HasStandardOptions = hasStandardOptions;
            RplDataResult.RplPriceReductionError = rplPriceReductionError;
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

        internal WhenRecognisingPriorLearningFixture EnterRplData(PriorLearningDataViewModel model)
        {
            DataViewModel.TrainingTotalHours = model.TrainingTotalHours;
            DataViewModel.DurationReducedByHours = model.DurationReducedByHours;
            DataViewModel.IsDurationReducedByRpl = model.IsDurationReducedByRpl;
            DataViewModel.DurationReducedBy = model.DurationReducedBy;
            DataViewModel.CostBeforeRpl = model.CostBeforeRpl;
            DataViewModel.PriceReduced = model.PriceReduced;
            return this;
        }
    }
}