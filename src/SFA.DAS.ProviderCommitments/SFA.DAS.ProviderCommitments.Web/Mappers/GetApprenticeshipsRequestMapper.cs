using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using ApiRequests = SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class GetApprenticeshipsRequestMapper : IMapper<GetApprenticeshipsRequest, ManageApprenticesViewModel>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel> _mapper;

        public GetApprenticeshipsRequestMapper(ICommitmentsApiClient client, IMapper<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDetailsViewModel> mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<ManageApprenticesViewModel> Map(GetApprenticeshipsRequest source)
        {
            var response = await _client.GetApprenticeships(new ApiRequests.GetApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                PageNumber = source.PageNumber,
                PageItemCount = source.PageItemCount,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
                EmployerName = source.SelectedEmployer,
                CourseName = source.SelectedCourse,
                Status = source.SelectedStatus,
                StartDate = source.SelectedStartDate,
                EndDate = source.SelectedEndDate
            });

            var filters = new GetApprenticeshipsFilterValuesResponse();
            
            if (response.TotalApprenticeships >= Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch)
            {
                filters = await _client.GetApprenticeshipsFilterValues(source.ProviderId);
            }

            var statusFilters = new[]
            {
                ApprenticeshipStatus.WaitingToStart, 
                ApprenticeshipStatus.Live,
                ApprenticeshipStatus.Paused, 
                ApprenticeshipStatus.Stopped
            };

            var filterModel = new ApprenticesFilterModel
            {
                TotalNumberOfApprenticeships = response.TotalApprenticeships,
                TotalNumberOfApprenticeshipsFound = response.TotalApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = response.TotalApprenticeshipsWithAlertsFound,
                PageNumber = source.PageNumber,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
                SelectedEmployer = source.SelectedEmployer,
                SelectedCourse = source.SelectedCourse,
                SelectedStatus = source.SelectedStatus,
                SelectedStartDate = source.SelectedStartDate,
                SelectedEndDate = source.SelectedEndDate,
                EmployerFilters = filters.EmployerNames,
                CourseFilters = filters.CourseNames,
                StatusFilters = statusFilters,
                StartDateFilters = filters.StartDates,
                EndDateFilters = filters.EndDates
            };
            
            var apprenticeships = new List<ApprenticeshipDetailsViewModel>();
            foreach (var apprenticeshipDetailsResponse in response.Apprenticeships)
            {
                var apprenticeship = await _mapper.Map(apprenticeshipDetailsResponse);
                apprenticeships.Add(apprenticeship);
            }

            return new ManageApprenticesViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships = apprenticeships,
                FilterModel = filterModel,
            };
        }
    }
}
