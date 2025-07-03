using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests;

public class WhenPostingEditApprenticeshipDetails
{
    private Fixture _autoFixture;
    private WhenPostingEditApprenticeshipDetailsFixture _fixture;
    private EditApprenticeshipRequestViewModel _viewModel;

    private GetApprenticeshipResponse _apprenticeshipResponse;
    private GetTrainingProgrammeResponse _standardVersionResponse;
    private GetTrainingProgrammeResponse _frameworkResponse;

    [SetUp]
    public void Arrange()
    {
        _autoFixture = new Fixture();
        _fixture = new WhenPostingEditApprenticeshipDetailsFixture();

        _apprenticeshipResponse = _autoFixture.Build<GetApprenticeshipResponse>()
            .With(x => x.CourseCode, _autoFixture.Create<int>().ToString())
            .Create();

        _standardVersionResponse = _autoFixture.Build<GetTrainingProgrammeResponse>()
            .With(x => x.TrainingProgramme, _autoFixture.Build<TrainingProgramme>()
                .With(x => x.Version, "1.0")
                .With(x => x.Options, new List<string>())
                .Create())
            .Create();

        _frameworkResponse = _autoFixture.Create<GetTrainingProgrammeResponse>();
        _frameworkResponse.TrainingProgramme.Version = null;

        _viewModel = _autoFixture.Build<EditApprenticeshipRequestViewModel>()
            .Without(x => x.StartDate)
            .Without(x => x.EndDate)
            .Without(x => x.DateOfBirth)
            .With(x => x.CourseCode, _apprenticeshipResponse.CourseCode)
            .Create();

        _viewModel.StartDate = new MonthYearModel(_apprenticeshipResponse.StartDate.Value.ToString("MMyyyy"));

        _fixture.SetUpGetApprenticeship(_apprenticeshipResponse);
    }

    [Test]
    public async Task And_NewStandardSelected_Then_EditApprenticeship_IsCalled()
    {
        _viewModel.CourseCode = _autoFixture.Create<int>().ToString();
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);

        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task And_StandardIsSelected_And_StartDateMovedForward_Then_EditApprenticeship_IsCalled()
    {
        _viewModel.StartDate = new MonthYearModel(_viewModel.StartDate.Date.Value.AddMonths(1).ToString("MMyyy"));
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);

        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task And_StandardNotChanged_And_StartDateNotMovedForward_Then_EditApprenticeship_IsCalled()
    {
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);
            
        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task And_StartDateIsMovedForward_And_FrameworkNotChanged_Then_EditApprenticeship_IsCalled()
    {
        _viewModel.StartDate = new MonthYearModel(_viewModel.StartDate.Date.Value.AddMonths(1).ToString("MMyyy"));
        _viewModel.CourseCode = "1-2-3";
        _apprenticeshipResponse.CourseCode = "1-2-3";
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);

        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task And_FrameworkIsChanged_Then_EditApprenticeship_IsCalled()
    {
        _viewModel.CourseCode = "4-5-6";
        _apprenticeshipResponse.CourseCode = "1-2-3";
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);

        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task And_FrameworkNotChanged_And_StartDateNotMovedForward_Then_EditApprenticeship_IsCalled()
    {
        _viewModel.CourseCode = "1-2-3";
        _apprenticeshipResponse.CourseCode = "1-2-3";
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);

        await _fixture.EditApprenticeship(_viewModel);

        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task VerifyValidationApiIsCalled()
    {
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);
            
        await _fixture.EditApprenticeship(_viewModel);
            
        _fixture.VerifyEditApprenticeshipIsCalled();
    }

    [Test]
    public async Task VerifyMapperIsCalled()
    {
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);
            
        await _fixture.EditApprenticeship(_viewModel);
            
        _fixture.VerifyMapperIsCalled();
    }

    [Test]
    public async Task And_StandardVersionHasNoOptions_VerifyRedirectedToConfirmEditApprenticeship()
    {
        _viewModel.HasOptions = false;
        _fixture.SetUpEditApprenticeshipWithHasOptions(false);
            
        var result = await _fixture.EditApprenticeship(_viewModel);
            
        WhenPostingEditApprenticeshipDetailsFixture.VerifyRedirectedToConfirmEditApprenticeship(result);
    }

    [Test]
    public async Task And_NewStandardVersionHasOptions_VerifyRedirectedToChangeOption()
    {
        _viewModel.CourseCode = _autoFixture.Create<int>().ToString();
        _viewModel.HasOptions = true;
        _standardVersionResponse.TrainingProgramme.Options = _autoFixture.Create<List<string>>();
        _fixture.SetUpEditApprenticeshipWithHasOptions(true);

        var result = await _fixture.EditApprenticeship(_viewModel);

        WhenPostingEditApprenticeshipDetailsFixture.VerifyRedirectedToChangeOption(result);
    }

    [Test]
    public async Task AndSelectCourseIsToBeChangedThenTheUserIsRedirectedToSelectCoursePage()
    {
        var result = await _fixture.EditChangingCourse(_viewModel);
        WhenPostingEditApprenticeshipDetailsFixture.VerifyRedirectedTo(result, "EditApprenticeshipCourse");
    }

    [Test]
    public async Task AndSelectDeliveryModelIsToBeChangedThenTheUserIsRedirectedToSelectDeliveryModelPage()
    {
        var result = await _fixture.EditChangingDeliveryModel(_viewModel);
        WhenPostingEditApprenticeshipDetailsFixture.VerifyRedirectedTo(result, "SelectDeliveryModelForEdit");
    }
}

public class WhenPostingEditApprenticeshipDetailsFixture : ApprenticeControllerTestFixtureBase
{
    private readonly ValidateEditApprenticeshipResponse _editApprenticeshipResponse;

    public WhenPostingEditApprenticeshipDetailsFixture()
    {
        Controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
        _editApprenticeshipResponse = new ValidateEditApprenticeshipResponse
        {
            ApprenticeshipId = 123,
            HasOptions = false,
            NeedReapproval = false
        };
    }

    public async Task<IActionResult> EditApprenticeship(EditApprenticeshipRequestViewModel viewModel)
    {
        return await Controller.EditApprenticeship(null, null, viewModel);
    }

    public async Task<IActionResult> EditChangingCourse(EditApprenticeshipRequestViewModel viewModel)
    {
        return await Controller.EditApprenticeship("Edit", null, viewModel);
    }

    public async Task<IActionResult> EditChangingDeliveryModel(EditApprenticeshipRequestViewModel viewModel)
    {
        return await Controller.EditApprenticeship(null, "Edit", viewModel);
    }

    public void SetUpGetApprenticeship(GetApprenticeshipResponse response)
    {
        MockCommitmentsApiClient.Setup(c => c.GetApprenticeship(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
    }

    public void SetUpEditApprenticeshipWithHasOptions(bool hasOptions)
    {
        var response = new ValidateEditApprenticeshipResponse
        {
            ApprenticeshipId = 123,
            HasOptions = hasOptions,
            NeedReapproval = false
        };
            
        MockOuterApiService.Setup(x => x.EditApprenticeship(
                It.IsAny<long>(), 
                It.IsAny<long>(), 
                It.IsAny<Infrastructure.OuterApi.Requests.Apprentices.ValidateEditApprenticeshipRequest>()))
            .ReturnsAsync(response);
    }

    public void SetUpGetCalculatedTrainingProgrammeVersion(EditApprenticeshipRequestViewModel viewModel, GetTrainingProgrammeResponse response)
    {
        MockCommitmentsApiClient.Setup(c => c.GetCalculatedTrainingProgrammeVersion(int.Parse(viewModel.CourseCode), viewModel.StartDate.Date.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
    }

    public void SetUpGetTrainingProgramme(EditApprenticeshipRequestViewModel viewModel, GetTrainingProgrammeResponse response)
    {
        MockCommitmentsApiClient.Setup(c => c.GetTrainingProgramme(viewModel.CourseCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
    }

    public void VerifyEditApprenticeshipIsCalled()
    {
        MockOuterApiService.Verify(x => x.EditApprenticeship(
                It.IsAny<long>(), 
                It.IsAny<long>(), 
                It.IsAny<Infrastructure.OuterApi.Requests.Apprentices.ValidateEditApprenticeshipRequest>()), 
            Times.Once());
    }

    public void VerifyGetCalculatedTrainingProgrameVersionIsCalled()
    {
        MockCommitmentsApiClient.Verify(x => x.GetCalculatedTrainingProgrammeVersion(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    public void VerifyGetTrainingProgrameIsCalled()
    {
        MockCommitmentsApiClient.Verify(x => x.GetTrainingProgramme(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    public void VerifyGetCalculatedTrainingProgrameVersionIsNotCalled()
    {
        MockCommitmentsApiClient.Verify(x => x.GetCalculatedTrainingProgrammeVersion(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    public void VerifyGetTrainingProgrameIsNotCalled()
    {
        MockCommitmentsApiClient.Verify(x => x.GetTrainingProgramme(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    public void VerifyMapperIsCalled()
    {
        MockMapper.Verify(x => x.Map<Infrastructure.OuterApi.Requests.Apprentices.ValidateEditApprenticeshipRequest>(It.IsAny<EditApprenticeshipRequestViewModel>()), Times.Once());
    }

    public static void VerifyRedirectedToConfirmEditApprenticeship(IActionResult actionResult)
    {
        actionResult.VerifyReturnsRedirectToActionResult().WithActionName("ConfirmEditApprenticeship");
    }

    public static void VerifyRedirectedToChangeOption(IActionResult actionResult)
    {
        actionResult.VerifyReturnsRedirectToActionResult().WithActionName("ChangeOption");
    }

    public static void VerifyRedirectedTo(IActionResult actionResult, string actionName)
    {
        actionResult.VerifyReturnsRedirectToActionResult().WithActionName(actionName);
    }
}