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
        
        [Test]
        public async Task AndWhenWhenUserSelectsToContactTheEmployer()
        {
            await _fixture.SetupStartDraftOverlapOptions(OverlapOptions.ContactTheEmployer)
                .DraftApprenticeshipOverlapOptionsChangeEmployer();
            
            _fixture.VerifyOverlappingTrainingDateRequestEmail_IsNotSent();
            _fixture.VerifyUserRedirectedTo(ControllerConstants.ApprenticeController.Actions.Index,  ControllerConstants.ApprenticeController.Name);
        }
        
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