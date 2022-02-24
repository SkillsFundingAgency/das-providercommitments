using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

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
        public GetBulkUploadAddDraftApprenticeshipsResponse DraftApprenticeshipsResponse { get; set; }

        public BulkUploadAddDraftApprenticeshipsViewModel _result { get; set; }

        public BulkUploadAddDraftApprenticeshipsViewModelMapper _sut { get; set; }

        public string cohortReference = "MKRK7V";
        public string EmployerName = "Tesco";
        public int NumberOfApprenticeships = 2;        

        public WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture()
        {
            _sut = new BulkUploadAddDraftApprenticeshipsViewModelMapper();
        }

        public async Task Action() => _result = await _sut.Map(DraftApprenticeshipsResponse);

        public WhenMappingBulkUploadAddDraftApprenticeshipsResponseToViewModelFixture Map()
        {
            DraftApprenticeshipsResponse = new GetBulkUploadAddDraftApprenticeshipsResponse()
            {
                BulkUploadAddDraftApprenticeshipsResponse = new List<BulkUploadAddDraftApprenticeshipsResponse>()
                    {  
                        new BulkUploadAddDraftApprenticeshipsResponse()
                        {                            
                            CohortReference = "MKRK7V", 
                            EmployerName = "Tesco", 
                            NumberOfApprenticeships = 1
                        },
                       new BulkUploadAddDraftApprenticeshipsResponse()
                       {
                            CohortReference = "MKRK7V",
                            EmployerName = "Tesco",
                            NumberOfApprenticeships = 1
                       },
                       new BulkUploadAddDraftApprenticeshipsResponse()
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
            var responseEmployerName = DraftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().EmployerName;
            var viewModelEmployerName = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().EmployerName;
            
            Assert.AreEqual(responseEmployerName, viewModelEmployerName);
        }

        public void VerifyCohortReferenceIsMappedCorrectly()
        {            
            var responseCohortReference = DraftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().CohortReference;
            var viewModelCohortReference = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().CohortReference;

            Assert.AreEqual(responseCohortReference, viewModelCohortReference);
        }

        public void VerifyNumberOfApprenticeshipIsMappedCorrectly()
        {           
            var responsenumberOfApprenticeships = DraftApprenticeshipsResponse.BulkUploadAddDraftApprenticeshipsResponse.FirstOrDefault().NumberOfApprenticeships;
            var viewModelnumberOfApprenticeships = _result.BulkUploadDraftApprenticeshipsViewModel.FirstOrDefault().NumberOfApprenticeships;

            Assert.AreNotEqual(responsenumberOfApprenticeships, viewModelnumberOfApprenticeships);
        }

        internal void VerifyCorrectNumberOfEmployersAreMapped()
        {
            Assert.AreEqual(2, _result.BulkUploadDraftApprenticeshipsViewModel.Count());
        }
    }
}
