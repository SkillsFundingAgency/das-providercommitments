using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SFA.DAS.ProviderCommitments.Web.Services;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditApprenticeshipCourseViewModelMapper : IMapper<EditApprenticeshipRequest, EditApprenticeshipCourseViewModel>
    {
        private readonly ITempDataStorageService _tempData;
        private readonly IOuterApiClient _apiClient;
        private const string ViewModelForEdit = "ViewModelForEdit";

        public EditApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData, IOuterApiClient apiClient)
        {
            _tempData = tempData;
            _apiClient = apiClient;
        }

        public async Task<EditApprenticeshipCourseViewModel> Map(EditApprenticeshipRequest source)
        {
            var data = _tempData.RetrieveFromCache<EditApprenticeshipRequestViewModel>(ViewModelForEdit);

            var apiRequest = new GetEditApprenticeshipCourseRequest(source.ProviderId, source.ApprenticeshipId);
            var apiResponse = await _apiClient.Get<GetEditApprenticeshipCourseResponse>(apiRequest);

            var result = new EditApprenticeshipCourseViewModel
            {
                CourseCode = data.CourseCode,
                ApprenticeshipHashedId = data.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                EmployerName = apiResponse.EmployerName,
                ShowManagingStandardsContent = apiResponse.IsMainProvider,
                Standards = apiResponse.Standards.Select(x => new Standard { CourseCode = x.CourseCode, Name = x.Name })
            };

            return result;
        }
    }
}
