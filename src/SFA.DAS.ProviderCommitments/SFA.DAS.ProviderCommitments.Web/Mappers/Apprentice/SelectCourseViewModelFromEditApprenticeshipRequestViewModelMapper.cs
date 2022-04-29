using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectCourseViewModelFromEditApprenticeshipRequestViewModelMapper : IMapper<EditApprenticeshipRequestViewModel, SelectCourseViewModel>    {
        private readonly ISelectCourseViewModelMapperHelper _helper;
        private readonly ICommitmentsApiClient _client;

        public SelectCourseViewModelFromEditApprenticeshipRequestViewModelMapper(ISelectCourseViewModelMapperHelper helper, ICommitmentsApiClient client)
        {
            _helper = helper;
            _client = client;
        }

        public async Task<SelectCourseViewModel> Map(EditApprenticeshipRequestViewModel source)
        {
            var apprenticeship = await _client.GetApprenticeship(source.ApprenticeshipId);
            var cohortDetail = await _client.GetCohort(apprenticeship.CohortId);

            return await _helper.Map(source.CourseCode, cohortDetail.AccountLegalEntityId);
        }
    }
}