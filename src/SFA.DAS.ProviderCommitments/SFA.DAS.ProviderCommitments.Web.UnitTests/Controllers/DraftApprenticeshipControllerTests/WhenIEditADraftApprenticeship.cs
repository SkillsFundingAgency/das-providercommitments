﻿using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.DraftApprenticeshipControllerTests
{
    [TestFixture]
    public class WhenIEditADraftApprenticeship
    {
        private DraftApprenticeshipControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new DraftApprenticeshipControllerTestFixture();
        }

        [Test]
        public async Task ShouldReturnEditDraftApprenticeshipView()
        {
            _fixture.SetupCommitmentsApiToReturnADraftApprentice();
            await _fixture.EditDraftApprenticeship();
            _fixture.VerifyEditDraftApprenticeshipViewModelIsSentToViewResult();
        }

        [Test]
        public async Task AndWhenSavingAndNoStandardOptionsTheDraftApprenticeIsSuccessful()
        {
            _fixture.SetUpStandardToReturnNoOptions().SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }

        [Test]
        public void AndWhenSavingFailsDueToValidationItShouldThrowCommitmentsApiModelException()
        {
            _fixture.SetupUpdatingToThrowCommitmentsApiException();
            Assert.ThrowsAsync<CommitmentsApiModelException>(async () => await _fixture.PostToEditDraftApprenticeship());
        }

        [Test]
        public async Task AndWhenSavesRedirectsToSelectOptionsViewIfHasOptionsAndIdIsDifferent()
        {
            _fixture.SetUpStandardToReturnOptions()
                .SetNewStandardSelected()
                .SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectToSelectOptionsPage();
        }

        [Test]
        public async Task AndWhenOptionsButStandardIsTheSameThenRedirectsToCohortDetails()
        {
            _fixture
                .SetUpStandardToReturnOptions()
                .SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }
        
        [Test]
        public async Task AndWhenNoOptionsButStandardIsDifferentThenRedirectsToCohortDetails()
        {
            _fixture
                .SetNewStandardSelected()
                .SetUpStandardToReturnNoOptions()
                .SetupCommitmentsApiToReturnADraftApprentice();
            
            await _fixture.PostToEditDraftApprenticeship();
            _fixture.VerifyUpdateMappingToApiTypeIsCalled()
                .VerifyApiUpdateMethodIsCalled()
                .VerifyRedirectedBackToCohortDetailsPage();
        }
    }
}