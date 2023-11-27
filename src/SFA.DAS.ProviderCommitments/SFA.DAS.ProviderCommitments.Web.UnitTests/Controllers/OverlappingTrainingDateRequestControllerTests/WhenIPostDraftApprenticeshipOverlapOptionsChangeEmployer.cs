using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Controllers;
using StructureMap.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Controllers.OverlappingTrainingDateRequestControllerTests
{
    [TestFixture]
    public class WhenIPostDraftApprenticeshipOverlapOptionsChangeEmployer
    {
        private OverlappingTrainingDateRequestControllerTestFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new OverlappingTrainingDateRequestControllerTestFixture();
        }

        [Test]
        public async Task AndWhenWhenUserSelectsToSendOverlapEmailToEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest)
                .DraftApprenticeshipOverlapOptionsChangeEmployer();
            
            _fixture.VerifyOverlappingTrainingDateRequestEmailSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.OverlappingTrainingDateRequestController.Actions.EmployerNotified);
        }
        
        // [Test]
        // public async Task AndWhenExistingDraftApprenticeship_AndUserSelectsToSendOverlapEmailToEmployer_ThenUpdateRecord()
        // {
        //     await _fixture
        //         .SetupStartDraftOverlapOptions(OverlapOptions.SendStopRequest)
        //         .SetupUpdateDraftApprenticeshipRequestMapper()
        //         .SetupGetTempEditDraftApprenticeship()
        //         .WithCohortReference()
        //         .WithDraftApprenticeshipHashedId()
        //         .DraftApprenticeshipOverlapOptions();
        //     
        //     _fixture.VerifyExistingDraftApprenticeshipUpdated();
        // }
        
        // [Test]
        // public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        // {
        //     await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer).DraftApprenticeshipOverlapOptions();
        //     _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
        //     _fixture.VerifyUserRedirectedTo("Details");
        // }
        
        [Test]
        public async Task AndWhenWhenUserSelectsToAddApprenticeshipLater()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.AddApprenticeshipLater)
                .DraftApprenticeshipOverlapOptionsChangeEmployer();
            
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.ApprenticeController.Actions.Index,  ControllerConstants.ApprenticeController.Name);
        }
    }
}