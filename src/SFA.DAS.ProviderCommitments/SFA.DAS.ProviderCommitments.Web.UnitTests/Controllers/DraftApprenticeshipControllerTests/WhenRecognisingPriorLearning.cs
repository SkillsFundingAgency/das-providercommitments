using System;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.ProviderCommitments.Web.Authentication;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests;

public class WhenRecognisingPriorLearning
{
    [Test]
    public async Task When_Get_Recognise_Prior_Learning_And_Rpl_Required_Return_View()
    {
        var fixture = new WhenRecognisingPriorLearningFixture();

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyReturnsViewModel().ViewName.Should().Be("RecognisePriorLearning");
    }
    
    [Test]
    public async Task When_Get_Recognise_Prior_Learning_And_Rpl_Not_Required_Redirect()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithRplNotRequired();

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyReturnsRedirectToActionResult();
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

    [Test]
    public async Task When_before_Aug_2022_then_does_not_require_rpl()
    {
        var fixture = new WhenRecognisingPriorLearningFixture().WithActualStartDate(new DateTime(2022, 07, 30));

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyRedirectsToCohortDetailsPage(fixture.Request.ProviderId, fixture.Request.CohortReference);
    }

    [Test]
    public async Task When_startdate_are_null_then_does_not_require_rpl()
    {
        var fixture = new WhenRecognisingPriorLearningFixture().WithActualStartDate(null).WithoutStartDate();

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyRedirectsToCohortDetailsPage(fixture.Request.ProviderId, fixture.Request.CohortReference);
    }

    [Test]
    public async Task When_ActualStartDate_after_Aug_2022_then_does_require_rpl()
    {
        var fixture = new WhenRecognisingPriorLearningFixture().WithActualStartDate(new DateTime(2022, 08, 30));

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyReturnsViewModel().WithModel<RecognisePriorLearningViewModel>()
            .RplNeedsToBeConsidered.Should().Be(true);
    }

    [Test]
    public async Task When_StartDate_after_Aug_2022_then_does_require_rpl()
    {
        var fixture = new WhenRecognisingPriorLearningFixture().WithActualStartDate(null).WithStartDate(2022,8);

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyReturnsViewModel().WithModel<RecognisePriorLearningViewModel>()
            .RplNeedsToBeConsidered.Should().Be(true);
    }

    [Test]
    public async Task When_StartDate_before_Aug_2022_then_does_not_require_rpl()
    {
        var fixture = new WhenRecognisingPriorLearningFixture().WithActualStartDate(null).WithStartDate(2022, 7);

        var result = await fixture.Sut.RecognisePriorLearning(fixture.Request);

        result.VerifyRedirectsToCohortDetailsPage(fixture.Request.ProviderId, fixture.Request.CohortReference);
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
    public async Task When_previously_rpl_data_exist_then_map_them(int? totalHours, int? hoursReducedBy,
        int? costBeforeRpl, int? priceReducedBy)
    {
        var model = new PriorLearningDataViewModel
        {
            TrainingTotalHours = totalHours,
            DurationReducedByHours = hoursReducedBy,
            CostBeforeRpl = costBeforeRpl,
            PriceReduced = priceReducedBy
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
                It.Is<CreatePriorLearningDataRequest>(r =>
                    r.TrainingTotalHours == model.TrainingTotalHours &&
                    r.DurationReducedByHours == model.DurationReducedByHours &&
                    r.IsDurationReducedByRpl == model.IsDurationReducedByRpl &&
                    r.DurationReducedBy == model.DurationReducedBy &&
                    r.CostBeforeRpl == model.CostBeforeRpl &&
                    r.PriceReducedBy == model.PriceReduced
                )));
    }

    [Test]
    public async Task
        When_user_selects_Yes_for_IsReducedByRpl_and_enters_a_value_then_decides_No_ensure_we_blank_the_hidden_value()
    {
        var model = new PriorLearningDataViewModel
        {
            TrainingTotalHours = 1000,
            DurationReducedByHours = 100,
            IsDurationReducedByRpl = false,
            DurationReducedBy = 8,
            PriceReduced = 1000
        };

        var fixture = new WhenRecognisingPriorLearningFixture()
            .EnterRplData(model);

        await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

        fixture.OuterApiService.Verify(x =>
            x.UpdatePriorLearningData(
                fixture.DataViewModel.ProviderId,
                fixture.DataViewModel.CohortId,
                fixture.DataViewModel.DraftApprenticeshipId,
                It.Is<CreatePriorLearningDataRequest>(r =>
                    r.TrainingTotalHours == model.TrainingTotalHours &&
                    r.DurationReducedByHours == model.DurationReducedByHours &&
                    r.IsDurationReducedByRpl == model.IsDurationReducedByRpl &&
                    r.DurationReducedBy == null &&
                    r.PriceReducedBy == model.PriceReduced
                )));
    }

    [Test]
    public async Task When_accessing_RecognisePriorLearningData_if_is_in_rpl_enhanced_mode()
    {
        var fixture = new WhenRecognisingPriorLearningFixture();

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.Request);

        var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDataViewModel>();
    }

    [Test]
    public async Task
        When_accessing_RecognisePriorLearningData_and_old_rpl_data_rpl_duration_is_0_then_change_to_null_and_False()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithRpl1Data(0, 1000);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.Request);

        var model = result.VerifyReturnsViewModel().WithModel<PriorLearningDataViewModel>();
        model.IsDurationReducedByRpl.Should().BeFalse();
        model.DurationReducedBy.Should().BeNull();
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
                It.Is<CreatePriorLearningDataRequest>(r =>
                    r.TrainingTotalHours == model.TrainingTotalHours &&
                    r.DurationReducedByHours == model.DurationReducedByHours &&
                    r.IsDurationReducedByRpl == model.IsDurationReducedByRpl &&
                    r.DurationReducedBy == model.DurationReducedBy &&
                    r.CostBeforeRpl == model.CostBeforeRpl &&
                    r.PriceReducedBy == model.PriceReduced
                )));
    }

    [Test]
    public async Task
        When_submitting_RPL_data_which_hold_a_value_inside_the_IsDurationReducedByRpl_but_that_field_is_set_to_No()
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
                It.Is<CreatePriorLearningDataRequest>(r =>
                    r.IsDurationReducedByRpl == false &&
                    r.DurationReducedBy == null
                )));
    }

    [TestCase, MoqAutoData]
    public async Task After_submitting_prior_learning_bad_data_then_show_RPL_summary_page()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithRplCreatePriorLearningDataResponse(false, true);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

        result.VerifyRedirectsToRecognisePriorLearningSummaryPage(
            fixture.DataViewModel.DraftApprenticeshipHashedId);
    }

    [TestCase, MoqAutoData]
    public async Task After_submitting_prior_learning_data_and__no_rpl_error_then_dont_show_RPL_summary_page_goto_cohort_overview()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithoutStandardOptions()
            .WithRplSummary(false, false)
            .WithRplCreatePriorLearningDataResponse(true, false);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

        result.VerifyRedirectsToCohortDetailsPage(fixture.DataViewModel.ProviderId, fixture.DataViewModel.CohortReference);
    }

    [TestCase, MoqAutoData]
    public async Task After_submitting_prior_learning_data_with_no_rpl_error_then_dont_show_RPL_summary_page()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithStandardOptions()
            .WithRplCreatePriorLearningDataResponse(false, false);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

        result.VerifyRedirectsToCohortDetailsPage(fixture.DataViewModel.ProviderId, fixture.DataViewModel.CohortReference);
    }

    [TestCase, MoqAutoData]
    public async Task Check_PercentageTotalTraining_calculates()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithoutStandardOptions()
            .WithRplSummary(true, false);

        var result = await fixture.Sut.RecognisePriorLearningSummary(fixture.RplSummaryRequest);

        var model = result.VerifyReturnsViewModel().WithModel<PriorLearningSummaryViewModel>();

        model.PercentageTotalTraining.Should()
            .Be((double)model.DurationReducedByHours / (double)model.TrainingTotalHours * 100);
    }

    [TestCase, MoqAutoData]
    public async Task Check_PercentageMinimumFunding_calculates()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithoutStandardOptions()
            .WithRplSummary(true, false);

        var result = await fixture.Sut.RecognisePriorLearningSummary(fixture.RplSummaryRequest);

        var model = result.VerifyReturnsViewModel().WithModel<PriorLearningSummaryViewModel>();

        model.PercentageMinimumFunding.Should().Be(model.PercentageTotalTraining / 2);
    }

    [Test]
    public async Task When_getting_rpl_summary_page()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithoutStandardOptions()
            .WithRplSummary(true, false);

        var result = await fixture.Sut.RecognisePriorLearningSummary(fixture.RplSummaryRequest);

        var model = result.VerifyReturnsViewModel().WithModel<PriorLearningSummaryViewModel>();

        model.CohortId.Should().Be(fixture.RplSummaryRequest.CohortId);
        model.CohortReference.Should().Be(fixture.RplSummaryRequest.CohortReference);
        model.DraftApprenticeshipId.Should().Be(fixture.RplSummaryRequest.DraftApprenticeshipId);
        model.ProviderId.Should().Be(fixture.RplSummaryRequest.ProviderId);
        model.DraftApprenticeshipHashedId.Should().Be(fixture.RplSummaryRequest.DraftApprenticeshipHashedId);
        model.TrainingTotalHours.Should().Be(fixture.RplSummary.TrainingTotalHours);
        model.DurationReducedByHours.Should().Be(fixture.RplSummary.DurationReducedByHours);
        model.CostBeforeRpl.Should().Be(fixture.RplSummary.CostBeforeRpl);
        model.PriceReducedBy.Should().Be(fixture.RplSummary.PriceReducedBy);
        model.FundingBandMaximum.Should().Be(fixture.RplSummary.FundingBandMaximum);
        model.PercentageOfPriorLearning.Should().Be(fixture.RplSummary.PercentageOfPriorLearning);
        model.MinimumPercentageReduction.Should().Be(fixture.RplSummary.MinimumPercentageReduction);
        model.MinimumPriceReduction.Should().Be(fixture.RplSummary.MinimumPriceReduction);
        model.RplPriceReductionError.Should().Be(fixture.RplSummary.RplPriceReductionError);
        model.TotalCost.Should().Be(fixture.RplSummary.TotalCost);
        model.FullName.Should()
            .Be(string.Format("{0} {1}", fixture.RplSummary.FirstName, fixture.RplSummary.LastName));
        model.HasStandardOptions.Should().Be(fixture.RplSummary.HasStandardOptions);
    }

    [Test]
    public async Task After_submitting_prior_learning_data_and_error_then_show_blue_error_page()
    {
        var fixture = new WhenRecognisingPriorLearningFixture()
            .WithRplCreatePriorLearningDataResponse(false, true);

        var result = await fixture.Sut.RecognisePriorLearningData(fixture.DataViewModel);

        result.VerifyRedirectsToRecognisePriorLearningSummaryPage(fixture.DataViewModel
            .DraftApprenticeshipHashedId);
    }
}

public class WhenRecognisingPriorLearningFixture
{
    public DraftApprenticeshipController Sut { get; set; }

    private readonly GetDraftApprenticeshipResponse Apprenticeship;
    public GetPriorLearningSummaryQueryResult RplSummary;
    public RecognisePriorLearningRequest Request;
    public PriorLearningSummaryRequest RplSummaryRequest;
    public RecognisePriorLearningViewModel ViewModel;
    public PriorLearningDataViewModel DataViewModel;
    public CreatePriorLearningDataResponse RplCreatePriorLearningDataResponse;
    public GetPriorLearningDataQueryResult PriorLearningDataQueryResult;

    public Mock<IOuterApiService> OuterApiService;

    public RecognisePriorLearningResult RplDataResult;

    public Mock<ICommitmentsApiClient> ApiClient { get; }
    public Mock<IAuthorizationService> AuthorizationService { get; }

    public WhenRecognisingPriorLearningFixture()
    {
        var fixture = new Fixture();
        Request = fixture.Create<RecognisePriorLearningRequest>();
        RplSummaryRequest = fixture.Create<PriorLearningSummaryRequest>();
        ViewModel = fixture.Create<RecognisePriorLearningViewModel>();
        ViewModel.IsTherePriorLearning = true;
        DataViewModel = fixture.Build<PriorLearningDataViewModel>().Create();
        Apprenticeship = fixture.Create<GetDraftApprenticeshipResponse>();
        RplSummary = fixture.Create<GetPriorLearningSummaryQueryResult>();
        RplDataResult = fixture.Create<RecognisePriorLearningResult>();
        PriorLearningDataQueryResult = fixture.Create<GetPriorLearningDataQueryResult>();
        RplCreatePriorLearningDataResponse = fixture.Create<CreatePriorLearningDataResponse>();
        fixture.Create<CreatePriorLearningDataRequest>();

        ApiClient = new Mock<ICommitmentsApiClient>();
        ApiClient.Setup(x =>
                x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Apprenticeship);
        
        OuterApiService = new Mock<IOuterApiService>();
        OuterApiService.Setup(x => x.GetPriorLearningSummary(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(RplSummary);
        OuterApiService
            .Setup(x => x.UpdatePriorLearningData(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(),
                It.IsAny<CreatePriorLearningDataRequest>())).ReturnsAsync(RplCreatePriorLearningDataResponse);
        OuterApiService.Setup(x => x.GetPriorLearningData(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(PriorLearningDataQueryResult);

        AuthorizationService = new Mock<IAuthorizationService>();

        Sut = new DraftApprenticeshipController(
            Mock.Of<IMediator>(),
            ApiClient.Object,
            new SimpleModelMapper(
                new RecognisePriorLearningRequestToViewModelMapper(ApiClient.Object, OuterApiService.Object),
                new RecognisePriorLearningRequestToDataViewModelMapper(OuterApiService.Object),
                new RecognisePriorLearningSummaryRequestToSummaryViewModelMapper(OuterApiService.Object),
                new RecognisePriorLearningViewModelToResultMapper(ApiClient.Object),
                new PriorLearningDataViewModelToResultMapper(OuterApiService.Object)),
            Mock.Of<IEncodingService>(),
            AuthorizationService.Object,
            OuterApiService.Object,
            Mock.Of<IAuthenticationService>(),
            Mock.Of<ICacheStorageService>());
    }

    internal WhenRecognisingPriorLearningFixture WithoutPreviousSelection()
    {
        Apprenticeship.RecognisePriorLearning = null;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithActualStartDate(DateTime? date )
    {
        Apprenticeship.ActualStartDate = date;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithStartDate(int year, int month)
    {
        Apprenticeship.StartDate = new DateTime(year, month, 1);
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithoutStartDate()
    {
        Apprenticeship.StartDate = null;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithPreviousSelection(bool previousSelection)
    {
        Apprenticeship.RecognisePriorLearning = previousSelection;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithPreviousDetails(int? durationReducedBy, int? priceReducedBy, int? durationReducedByHours)
    {
        Apprenticeship.DurationReducedBy = durationReducedBy;
        Apprenticeship.PriceReducedBy = priceReducedBy;
        Apprenticeship.DurationReducedByHours = durationReducedByHours;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture ChoosePriorLearning(bool? priorLearning)
    {
        ViewModel.IsTherePriorLearning = priorLearning;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithRpl1Data(int durationReducedBy, int priceReducedBy)
    {
        PriorLearningDataQueryResult.TrainingTotalHours = null;
        PriorLearningDataQueryResult.DurationReducedByHours = null;
        PriorLearningDataQueryResult.IsDurationReducedByRpl = null;
        PriorLearningDataQueryResult.DurationReducedBy = durationReducedBy;
        PriorLearningDataQueryResult.PriceReduced = priceReducedBy;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithoutStandardOptions()
    {
        RplCreatePriorLearningDataResponse.HasStandardOptions = false;
        Apprenticeship.HasStandardOptions = false;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithStandardOptions()
    {
        RplCreatePriorLearningDataResponse.HasStandardOptions = true;
        Apprenticeship.HasStandardOptions = true;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithRplSummary(bool rplPriceReductionError, bool hasStandardOptions)
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
        RplSummary.HasStandardOptions = hasStandardOptions;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithRplDataResult(bool hasStandardOptions, bool rplPriceReductionError)
    {
        RplCreatePriorLearningDataResponse.HasStandardOptions = hasStandardOptions;
        RplDataResult.RplPriceReductionError = rplPriceReductionError;
        return this;
    }

    internal WhenRecognisingPriorLearningFixture WithRplCreatePriorLearningDataResponse(bool hasStandardOptions, bool rplPriceReductionError)
    {
        RplCreatePriorLearningDataResponse.HasStandardOptions = hasStandardOptions;
        RplCreatePriorLearningDataResponse.RplPriceReductionError = rplPriceReductionError;
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

    internal WhenRecognisingPriorLearningFixture WithRplNotRequired()
    {
        Apprenticeship.CourseCode = "123";
        OuterApiService.Setup(x => x.GetRplRequirements(Request.ProviderId, Request.CohortId, Request.DraftApprenticeshipId, "123"))
            .ReturnsAsync(new GetRplRequirementsResponse { IsRequired = false });
        return this;
    }
}