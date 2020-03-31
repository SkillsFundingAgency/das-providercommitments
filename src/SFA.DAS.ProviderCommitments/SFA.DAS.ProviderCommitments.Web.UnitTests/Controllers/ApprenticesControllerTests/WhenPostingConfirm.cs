using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using StructureMap.Query;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.ApprenticesControllerTests
{
    [TestFixture]
    public class WhenPostingConfirm
    {
        private WhenPostingConfirmFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenPostingConfirmFixture();
        }

        [Test]
        public void PostConfirm_WithValidModel_RedirectToSent()
        {
            var result = _fixture.Act();
            result.VerifyReturnsRedirectToRouteResult().WithRouteName(RouteNames.ApprenticeSent);
        }

        internal class WhenPostingConfirmFixture
        {
            private readonly ApprenticeController _sut;
            private readonly ConfirmViewModel _viewModel;

            public WhenPostingConfirmFixture()
            {
                _viewModel = new ConfirmViewModel
                {
                    ApprenticeshipHashedId = "DF34WG2",
                    ProviderId = 2342,
                    EmployerAccountLegalEntityPublicHashedId = "DFF41G",
                    NewStartDate = new MonthYearModel("62020")
                };

                _sut = new ApprenticeController(Mock.Of<IModelMapper>(), Mock.Of<ICookieStorageService<IndexRequest>>());
            }

            public IActionResult Act() => _sut.Confirm(_viewModel);
        }

    }
}
