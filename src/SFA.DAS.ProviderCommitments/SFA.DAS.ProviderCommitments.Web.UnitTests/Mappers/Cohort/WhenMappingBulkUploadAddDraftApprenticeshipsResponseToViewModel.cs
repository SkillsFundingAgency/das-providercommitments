using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModel
    {
        [Test]
        public async Task EmployerNameIsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture();
            
            //Act
            await fixture.Map().Action();

            //Assert
            fixture.VerifyEmployerNameIsMappedCorrectly();
        }

        [Test]
        public async Task CohortReferenceIsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture();

            //Act
            await fixture.Map().Action();

            //Assert
            fixture.VerifyEmployerNameIsMappedCorrectly();
        }

        [Test]
        public async Task NumberOfApprenticeshipsIsMappedCorrectly()
        {
            //Arrange
            var fixture = new WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture();

            //Act
            await fixture.Map().Action();

            //Assert
            fixture.VerifyNumberOfApprenticeshipIsMappedCorrectly();
        }

        [Test]
        public async Task CorrectNumberOfEmployersAreMapped()
        {
            //Arrange
            var fixture = new WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture();

            //Act
            await fixture.Map().Action();

            //Assert
            fixture.VerifyCorrectNumberOfEmployersAreMapped();
        }

    }

    public class WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture
    {
        private GetBulkUploadAddDraftApprenticeshipsResponse _draftApprenticeshipsResponse;
        private BulkUploadAddDraftApprenticeshipsViewModel _result;
        private readonly BulkUploadAddDraftApprenticeshipsViewModelMapper _sut;
        
        public WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture()
        {
            _sut = new BulkUploadAddDraftApprenticeshipsViewModelMapper();
        }

        public async Task Action() => _result = await _sut.Map(_draftApprenticeshipsResponse);

        public WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture Map()
        {
            _draftApprenticeshipsResponse = new GetBulkUploadAddDraftApprenticeshipsResponse()
            {
                BulkUploadAddDraftApprenticeshipsResponse = new List<BulkUploadAddDraftApprenticeshipsResponse>()
                    {  
                        new()
                        {                            
                            CohortReference = "MKRK7V", 
                            EmployerName = "Tesco", 
                            NumberOfApprenticeships = 1
                        },
                       new()
                       {
                            CohortReference = "MKRK7V",
                            EmployerName = "Tesco",
                            NumberOfApprenticeships = 1
                       },
                       new()
                       {
                            CohortReference = "MKRK7N",
                            EmployerName = "Nasdaq",
                            NumberOfApprenticeships = 1
                       },
                    }
            };

            return this;
        }

        public void VerifyEmployerNameIsMappedCorrectly()
        {
            var responseEmployerName = _draftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().EmployerName;
            var viewModelEmployerName = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().EmployerName;

            Assert.That(viewModelEmployerName, Is.EqualTo(responseEmployerName));
        }

        public void VerifyCohortReferenceIsMappedCorrectly()
        {            
            var responseCohortReference = _draftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().CohortReference;
            var viewModelCohortReference = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().CohortReference;

            Assert.That(viewModelCohortReference, Is.EqualTo(responseCohortReference));
        }

        public void VerifyNumberOfApprenticeshipIsMappedCorrectly()
        {           
            var responsenumberOfApprenticeships = _draftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().NumberOfApprenticeships;
            var viewModelnumberOfApprenticeships = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().NumberOfApprenticeships;

            Assert.That(viewModelnumberOfApprenticeships, Is.EqualTo(responsenumberOfApprenticeships));
        }

        internal void VerifyCorrectNumberOfEmployersAreMapped()
        {
            Assert.That(_result.BulkUploadDraftApprenticeshipsViewModel, Has.Count.EqualTo(3));
        }
    }
}
