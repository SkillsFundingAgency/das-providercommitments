﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    public class WhenPostingChangeVersion
    {
        private ChangeVersionViewModel _viewModel;
        private EditApprenticeshipRequestViewModel _editRequestViewModel;

        private WhenPostingChangeVersionFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            var autoFixture = new Fixture();

            _viewModel = autoFixture.Create<ChangeVersionViewModel>();

            var baseDate = DateTime.Now;
            var startDate = new MonthYearModel(baseDate.ToString("MMyyyy"));
            var endDate = new MonthYearModel(baseDate.AddYears(2).ToString("MMyyyy"));
            var dateOfBirth = new MonthYearModel(baseDate.AddYears(-18).ToString("MMyyyy"));

            _editRequestViewModel = autoFixture.Build<EditApprenticeshipRequestViewModel>()
                    .With(x => x.StartDate, startDate)
                    .With(x => x.EndDate, endDate)
                    .With(x => x.DateOfBirth, dateOfBirth)
                .Create();

            _fixture = new WhenPostingChangeVersionFixture();

            _fixture.SetUpMockMapper(_editRequestViewModel);
        }

        [Test]
        public async Task VerifyMapperIsCalled()
        {
            await _fixture.ChangeVersion(_viewModel);

            _fixture.VerifyMapperIsCalled();
        }

        [Test]
        public async Task And_VersionHasOptions_Then_VerifyRedirectToChangeOption()
        {
            _editRequestViewModel.HasOptions = true;

            var result = await _fixture.ChangeVersion(_viewModel);

            result.VerifyReturnsRedirectToActionResult().WithActionName("ChangeOption");
        }

        [Test]
        public async Task And_VersionHasNoOptions_Then_VerifyRedirectToConfirmEditApprenticeship()
        {
            _editRequestViewModel.HasOptions = false;

            var result = await _fixture.ChangeVersion(_viewModel);

            _fixture.VerifyReturnToConfirmEditApprenticeship(result as RedirectToActionResult);
        }
    }

    public class WhenPostingChangeVersionFixture : ApprenticeControllerTestFixtureBase
    {
        public WhenPostingChangeVersionFixture() : base()
        {
            _controller.TempData = new TempDataDictionary(Mock.Of<HttpContext>(), Mock.Of<ITempDataProvider>());
        }

        public async Task<IActionResult> ChangeVersion(ChangeVersionViewModel viewModel)
        {
            return await _controller.ChangeVersion(viewModel);
        }

        public void SetUpMockMapper(EditApprenticeshipRequestViewModel editApprenticeshipViewModel)
        {
            _mockMapper.Setup(m => m.Map<EditApprenticeshipRequestViewModel>(It.IsAny<ChangeVersionViewModel>()))
                .ReturnsAsync(editApprenticeshipViewModel);
        }

        public void VerifyReturnToConfirmEditApprenticeship(RedirectToActionResult redirectResult)
        {
            redirectResult.ActionName.Should().Be("ConfirmEditApprenticeship");
        }

        public void VerifyMapperIsCalled()
        {
            _mockMapper.Verify(x => x.Map<EditApprenticeshipRequestViewModel>(It.IsAny<ChangeVersionViewModel>()), Times.Once());
        }
    }
}
