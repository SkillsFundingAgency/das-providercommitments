using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class GetApprenticeshipsRequestMapper : IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>
    {
        private readonly ICommitmentsApiClient _client;

        public GetApprenticeshipsRequestMapper(ICommitmentsApiClient client)
        {
            _client = client;
        }

        public async Task<ManageApprenticesViewModel> Map(GetApprenticeshipsRequest source)
        {
            var response = await _client.GetApprenticeships(new GetApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                PageNumber = source.PageNumber,
                PageItemCount = source.PageItemCount,
                //SearchTerm = source.SearchTerm,
                EmployerName = source.SelectedEmployer,
                CourseName = source.SelectedCourse,
                Status = source.SelectedStatus,
                StartDate = source.SelectedStartDate,
                EndDate = source.SelectedEndDate
            });

            var filters = new GetApprenticeshipsFilterValuesResponse();
            if (response.TotalApprenticeships >=
                ProviderCommitmentsWebConstants.NumberOfApprenticesRequiredForSearch || true)
            {
                filters = await _client.GetApprenticeshipsFilterValues(source.ProviderId);
            }

            var filterModel = new ManageApprenticesFilterModel
            {
                TotalNumberOfApprenticeships = response.TotalApprenticeships,
                TotalNumberOfApprenticeshipsWithAlerts = response.TotalApprenticeshipsWithAlerts,
                TotalNumberOfApprenticeshipsFound = response.TotalApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = response.TotalApprenticeshipsWithAlertsFound,
                PageNumber = source.PageNumber,
                SearchTerm = source.SearchTerm,
                SelectedEmployer = source.SelectedEmployer,
                SelectedCourse = source.SelectedCourse,
                SelectedStatus = source.SelectedStatus,
                SelectedStartDate = source.SelectedStartDate,
                SelectedEndDate = source.SelectedEndDate,
                EmployerFilters = filters.EmployerNames,
                CourseFilters = filters.CourseNames,
                StatusFilters = filters.Statuses,
                StartDateFilters = filters.StartDates,
                EndDateFilters = filters.EndDates
            };

            return new ManageApprenticesViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships = response.Apprenticeships,
                FilterModel = filterModel
            };
        }
    }
}
