using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingDeleteCohortRequestToDeleteCohortViewModel
    {
        [Test]
        public async Task Then_TheCohortReferenceIsMapped()
        {
            var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public async Task Then_EmployerNameIsMapped()
        {
            var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_EmployerName_Is_Mapped();
        }

        [Test]
        public async Task Then_NumberOfApprenticesAreMapped()
        {
            var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public async Task Then_ApprenticeshipTrainingProgrammeAreMappedCorrectly()
        {
            var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_ApprenticeshipTrainingProgrammeAreMappedCorrectly();
        }

        [Test]
        public async Task Then_ProviderId_IsMapped()
        {
            var fixture = new WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture();
            await fixture.Map();

            fixture.Verify_ProviderId_IsMapped();
        }
    }

    public class WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture
    {
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public DeleteCohortRequest DeleteCohortRequest { get; set; }
        public DeleteCohortViewModel DeleteCohortViewModel { get; set; }
        public DeleteCohortRequestViewModelMapper Mapper { get; set; }
        public GetCohortResponse GetCohortResponse { get; set; }
        public GetDraftApprenticeshipsResponse GetDraftApprenticeshipsResponse { get; set; }

        public long ProviderId => 1;
        public long CohortId => 22;

        public WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture()
        {
            DeleteCohortRequest = new DeleteCohortRequest { ProviderId = ProviderId, CohortId = CohortId, CohortReference = "XYZ" };
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            GetCohortResponse = CreateGetCohortResponse();
            GetDraftApprenticeshipsResponse = CreateGetDraftApprenticeships();
            CommitmentsApiClient.Setup(c => c.GetCohort(CohortId, CancellationToken.None)).ReturnsAsync(GetCohortResponse);
            CommitmentsApiClient.Setup(c => c.GetDraftApprenticeships(CohortId, CancellationToken.None)).ReturnsAsync(GetDraftApprenticeshipsResponse);

            Mapper = new DeleteCohortRequestViewModelMapper(CommitmentsApiClient.Object);
        }

        private GetDraftApprenticeshipsResponse CreateGetDraftApprenticeships()
        {
            return new GetDraftApprenticeshipsResponse
            {
                DraftApprenticeships = new List<DraftApprenticeshipDto>
                {
                    new()
                    {
                        Id = 1,
                        CourseName = "Course1"
                    },
                    new()
                    {
                        Id = 2,
                        CourseName = "Course1"
                    },
                    new()
                    {
                        Id = 3,
                        CourseName = "Course2"
                    }
                }
            };
        }

        public async Task<WhenMappingDeleteCohortRequestToDeleteCohortViewModelFixture> Map()
        {
            DeleteCohortViewModel = await Mapper.Map(DeleteCohortRequest);
            return this;
        }

        private GetCohortResponse CreateGetCohortResponse()
        {
            return new GetCohortResponse
            {
                CohortId = 2,
                AccountLegalEntityId = 3,
                ProviderName = "asdf",
                LegalEntityName = "Employer1",
                WithParty = Party.Provider,
            };
          
        }

        internal void Verify_CohortReference_Is_Mapped()
        {
            Assert.AreEqual(DeleteCohortRequest.CohortReference, DeleteCohortViewModel.CohortReference);
        }

        internal void Verify_EmployerName_Is_Mapped()
        {
            Assert.AreEqual(GetCohortResponse.LegalEntityName, DeleteCohortViewModel.EmployerAccountName);
        }

        internal void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(GetDraftApprenticeshipsResponse.DraftApprenticeships.Count, DeleteCohortViewModel.NumberOfApprenticeships);
        }

        internal void Verify_ApprenticeshipTrainingProgrammeAreMappedCorrectly()
        {
            Assert.AreEqual(2, DeleteCohortViewModel.ApprenticeshipTrainingProgrammes.Count);
            Assert.IsTrue(DeleteCohortViewModel.ApprenticeshipTrainingProgrammes.Any(x => x == "2 Course1"));
            Assert.IsTrue(DeleteCohortViewModel.ApprenticeshipTrainingProgrammes.Any(x => x == "1 Course2"));
        }

        internal void Verify_ProviderId_IsMapped()
        {
            Assert.AreEqual(DeleteCohortRequest.ProviderId  , DeleteCohortViewModel.ProviderId);
        }
    }
}
