﻿using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests;

[TestFixture]
public class WhenAddingACohortWithDraftApprentice
{
    [Test]
    public void AndRedirectTargetIsCourse_ThenRedirectedToSelectCourse()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetRedirectTarget(CreateCohortRedirectModel.RedirectTarget.SelectCourse);

        var result = fixture.Act() as RedirectToActionResult;

        result.ActionName.Should().Be("SelectCourse");
    }

    [Test]
    public void AndRedirectTargetIsHowTo_ThenRedirectedToHowTo()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetRedirectTarget(CreateCohortRedirectModel.RedirectTarget.SelectHowTo);

        var result = fixture.Act() as RedirectToActionResult;

        result.ActionName.Should().Be("SelectHowToAddApprentice");
    }

    [Test]
    public void AndRedirectTargetIsSelectLearner_ThenRedirectedToSelectLearner()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetRedirectTarget(CreateCohortRedirectModel.RedirectTarget.SelectLearner);

        var result = fixture.Act() as RedirectToActionResult;

        result.ActionName.Should().Be("SelectLearnerRecord");
        result.ControllerName.Should().Be("Learner");
    }

    [Test]
    public void AndRedirectTargetIsFlexiPayment_ThenRedirectedToFlexiPaymentPilot()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture().SetRedirectTarget(CreateCohortRedirectModel.RedirectTarget.ChooseFlexiPaymentPilotStatus);

        var result = fixture.Act() as RedirectToActionResult;

        result.ActionName.Should().Be("ChoosePilotStatus");
    }

    [Test]
    public async Task AndOnAddApprenticeshipPage_ThenReturnsView()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

        var result = await fixture.ActOnAddApprenticeship();

        result.VerifyReturnsViewModel().WithModel<AddDraftApprenticeshipViewModel>();
    }

    [Test]
    public async Task AndOnAddApprenticeshipPage_ThenMapperIsCalled()
    {
        var fixture = new WhenAddingACohortWithDraftApprenticeFixture();

        await fixture.ActOnAddApprenticeship();

        fixture.VerifyMapperWasCalled();
    }
}

public class WhenAddingACohortWithDraftApprenticeFixture
{
    private readonly CohortController _sut;
    private readonly Mock<IModelMapper> _modelMapper;
    private readonly CreateCohortWithDraftApprenticeshipRequest _request;
    private readonly Mock<ITempDataDictionary> _tempData;
    private readonly CreateCohortRedirectModel _redirectModel;

    public WhenAddingACohortWithDraftApprenticeFixture()
    {
        _request = new CreateCohortWithDraftApprenticeshipRequest
        {
            DeliveryModel = DeliveryModel.PortableFlexiJob,
            CourseCode = "ABC123"
        };

        _redirectModel = new CreateCohortRedirectModel { RedirectTo = CreateCohortRedirectModel.RedirectTarget.SelectCourse };
        var viewModel = new AddDraftApprenticeshipViewModel
        {
            DeliveryModel = DeliveryModel.Regular,
            CourseCode = "DIFF123"
        };
        _modelMapper = new Mock<IModelMapper>();
        _modelMapper.Setup(x => x.Map<AddDraftApprenticeshipViewModel>(_request)).ReturnsAsync(viewModel);
        _modelMapper.Setup(x => x.Map<CreateCohortRedirectModel>(_request)).ReturnsAsync(_redirectModel);

        _sut = new CohortController(
            Mock.Of<IMediator>(),
            _modelMapper.Object, 
            Mock.Of<ILinkGenerator>(), 
            Mock.Of<ICommitmentsApiClient>(),
            Mock.Of<IEncodingService>(),
            Mock.Of<IOuterApiService>(),
            Mock.Of<IAuthorizationService>(), 
            Mock.Of<ILogger<CohortController>>()
        );

        _tempData = new Mock<ITempDataDictionary>();
        _sut.TempData = _tempData.Object;
    }

    public void VerifyDraftApprenticeshipWasRestoredAndValuesSet(AddDraftApprenticeshipViewModel viewModel)
    {
        if (viewModel == null)
        {
            throw new Exception("View model has not been restored");
        }

        if (viewModel.DeliveryModel != _request.DeliveryModel || viewModel.CourseCode != _request.CourseCode)
        {
            throw new Exception("View model does not have CourseCode and DeliveryModel set correctly");
        }
    }

    public WhenAddingACohortWithDraftApprenticeFixture SetRedirectTarget(CreateCohortRedirectModel.RedirectTarget target)
    {
        _redirectModel.RedirectTo = target;
        return this;
    }

    public void VerifyMapperWasCalled()
    {
        _modelMapper.Verify(x => x.Map<AddDraftApprenticeshipViewModel>(_request));
    }

    public IActionResult Act() => _sut.AddNewDraftApprenticeship(_request).Result;
    public async Task<IActionResult> ActOnAddApprenticeship() => await _sut.AddDraftApprenticeship(_request);
}