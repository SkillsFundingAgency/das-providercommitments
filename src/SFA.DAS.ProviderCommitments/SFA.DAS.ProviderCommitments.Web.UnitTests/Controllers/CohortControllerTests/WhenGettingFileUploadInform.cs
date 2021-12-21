﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    public class WhenGettingFileUploadInform
    {
        [Test]
        public void ThenReturnsView()
        {
            var fixture = new WhenGettingFileUploadInformFixture();

            var result = fixture.Act();

            result.VerifyReturnsViewModel();
        }

        [Test]
        public void ThenProviderIdIsMapped()
        {
            var fixture = new WhenGettingFileUploadInformFixture();

            var viewResult = fixture.Act();

            var model = viewResult.VerifyReturnsViewModel().WithModel<FileUploadStartViewModel>();

            Assert.AreEqual(fixture.ProviderId, model.ProviderId);
        }
    }

    public class WhenGettingFileUploadInformFixture
    {
        public CohortController Sut { get; set; }

        private readonly SelectAddDraftApprenticeshipJourneyRequest _request;
        public readonly long ProviderId = 123;

        public WhenGettingFileUploadInformFixture()
        {
            _request = new SelectAddDraftApprenticeshipJourneyRequest { ProviderId = ProviderId };
            Sut = new CohortController(Mock.Of<IMediator>(), Mock.Of<IModelMapper>(), Mock.Of<ILinkGenerator>(), Mock.Of<ICommitmentsApiClient>(), Mock.Of<IFeatureTogglesService<ProviderFeatureToggle>>(), Mock.Of<IEncodingService>());
        }

        public IActionResult Act() => Sut.FileUploadInform(_request);
    }
}