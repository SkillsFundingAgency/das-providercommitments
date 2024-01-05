﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenAddingNewPrice
    {
        [Test]
        public async Task GetThenCallsPriceViewModelMapper()
        {
            var fixture = new WhenAddingNewPriceFixture();

            await fixture.Sut.Price(fixture.PriceRequest);

            fixture.VerifyPriceViewMapperWasCalled();
        }

        [Test]
        public async Task GetThenReturnsView()
        {
            var fixture = new WhenAddingNewPriceFixture();

            var result = await fixture.Sut.Price(fixture.PriceRequest) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(typeof(PriceViewModel), result.Model.GetType());
        }

        [Test]
        [TestCase(ApprenticeshipStatus.Stopped, "2020-06-15", "2021-06-15")]
        [TestCase(ApprenticeshipStatus.Stopped, "2020-06-15", "2019-06-15")]
        [TestCase(ApprenticeshipStatus.Live, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.Completed, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.Paused, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.WaitingToStart, "2020-06-15", null)]
        public async Task PostThenCallsConfirmRequestMapper(ApprenticeshipStatus status, DateTime startDate, DateTime? stopDate)
        {
            var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status, StartDate = startDate, StopDate = stopDate } };

            await fixture.Sut.Price(fixture.PriceViewModel);

            if (status == ApprenticeshipStatus.Stopped && fixture.PriceViewModel.StartDate > fixture.PriceViewModel.StopDate.Value)
            {
                fixture.VerifyConfirmRequestMapperWasCalled();
            }
            else
            {
                fixture.VerifyChangeOfEmployerOverlapAlertRequestWasCalled();
            }
        }

        [Test]
        [TestCase(ApprenticeshipStatus.Stopped, "2020-06-15", "2021-06-15")]
        [TestCase(ApprenticeshipStatus.Stopped, "2020-06-15", "2019-06-15")]
        [TestCase(ApprenticeshipStatus.Live, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.Completed, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.Paused, "2020-06-15", null)]
        [TestCase(ApprenticeshipStatus.WaitingToStart, "2020-06-15", null)]
        public async Task PostThenReturnsARedirectResult(ApprenticeshipStatus status, DateTime startDate, DateTime? stopDate)
        {
            var fixture = new WhenAddingNewPriceFixture { PriceViewModel = { ApprenticeshipStatus = status, StartDate = startDate, StopDate = stopDate } };

            var result = await fixture.Sut.Price(fixture.PriceViewModel) as RedirectToRouteResult;

            Assert.NotNull(result);

            if (status == ApprenticeshipStatus.Stopped && fixture.PriceViewModel.StartDate > fixture.PriceViewModel.StopDate.Value)
            {
                Assert.AreEqual(RouteNames.ApprenticeConfirm, result.RouteName);
            }
            else
            {
                Assert.AreEqual(RouteNames.ChangeEmployerOverlapAlert, result.RouteName);
            }
        }
    }

    public class WhenAddingNewPriceFixture
    {
        public ApprenticeController Sut { get; set; }
        public PriceRequest PriceRequest { get; set; }
        public PriceViewModel PriceViewModel { get; set; }
        public ConfirmRequest ConfirmRequest { get; set; }

        private readonly Mock<IModelMapper> _modelMapperMock;
        private readonly Fixture _fixture;

        public WhenAddingNewPriceFixture()
        {
            _fixture = new Fixture();
            PriceRequest = _fixture.Create<PriceRequest>();
            PriceViewModel = _fixture.Create<PriceViewModel>();
            ConfirmRequest = _fixture.Create<ConfirmRequest>();

            _modelMapperMock = new Mock<IModelMapper>();
            _modelMapperMock.Setup(x => x.Map<PriceViewModel>(It.IsAny<PriceRequest>()))
                .ReturnsAsync(PriceViewModel);
            _modelMapperMock.Setup(x => x.Map<ConfirmRequest>(It.IsAny<PriceViewModel>()))
                .ReturnsAsync(ConfirmRequest);

            Sut = new ApprenticeController(_modelMapperMock.Object, Mock.Of<ICookieStorageService<IndexRequest>>(),
                Mock.Of<ICommitmentsApiClient>(), Mock.Of<IOuterApiService>());
        }

        public void VerifyPriceViewMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<PriceViewModel>(PriceRequest));
        }

        public void VerifyConfirmRequestMapperWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ConfirmRequest>(PriceViewModel));
        }

        public void VerifyChangeOfEmployerOverlapAlertRequestWasCalled()
        {
            _modelMapperMock.Verify(x => x.Map<ChangeOfEmployerOverlapAlertRequest>(PriceViewModel));
        }
    }
}