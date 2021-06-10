using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class IndexViewModelMapper : IMapper<IndexRequest, IndexViewModel>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly IModelMapper _modelMapper;

        public IndexViewModelMapper(ICommitmentsApiClient client, IModelMapper modelMapper)
        {
            _client = client;
            _modelMapper = modelMapper;
        }

        public async Task<IndexViewModel> Map(IndexRequest source)
        {
            var response = await _client.GetApprenticeships(new GetApprenticeshipsRequest
            {
                ProviderId = source.ProviderId,
                PageNumber = source.PageNumber,
                PageItemCount = Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
                EmployerName = source.SelectedEmployer,
                CourseName = source.SelectedCourse,
                Status = source.SelectedStatus,
                StartDate = source.SelectedStartDate,
                EndDate = source.SelectedEndDate,
                Alert = source.SelectedAlert
            });

            var statusFilters = new[]
            {
                ApprenticeshipStatus.WaitingToStart, 
                ApprenticeshipStatus.Live,
                ApprenticeshipStatus.Paused, 
                ApprenticeshipStatus.Stopped,
                ApprenticeshipStatus.Completed
            };

            var alertFilters = new[]
            {
                Alerts.IlrDataMismatch,
                Alerts.ChangesPending,
                Alerts.ChangesRequested,
                Alerts.ChangesForReview 
            };

            var filterModel = new ApprenticesFilterModel
            {
                TotalNumberOfApprenticeships = response.TotalApprenticeships,
                TotalNumberOfApprenticeshipsFound = response.TotalApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = response.TotalApprenticeshipsWithAlertsFound,
                PageNumber = response.PageNumber,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
                SelectedEmployer = source.SelectedEmployer,
                SelectedCourse = source.SelectedCourse,
                SelectedStatus = source.SelectedStatus,
                SelectedStartDate = source.SelectedStartDate,
                SelectedEndDate = source.SelectedEndDate,
                StatusFilters = statusFilters,
                AlertFilters = alertFilters
            };

            if (response.TotalApprenticeships >= Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch)
            {
                var filters = await _client.GetApprenticeshipsFilterValues(
                    new GetApprenticeshipFiltersRequest{ProviderId = source.ProviderId});

                filterModel.EmployerFilters = filters.EmployerNames;
                filterModel.CourseFilters = filters.CourseNames;
                filterModel.StartDateFilters = filters.StartDates;
                filterModel.EndDateFilters = filters.EndDates;
            }

            var apprenticeships = new List<ApprenticeshipDetailsViewModel>();
            foreach (var apprenticeshipDetailsResponse in response.Apprenticeships)
            {
                var apprenticeship = await _modelMapper.Map<ApprenticeshipDetailsViewModel>(apprenticeshipDetailsResponse);
                apprenticeships.Add(apprenticeship);
            }

            return new IndexViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships = apprenticeships,
                FilterModel = filterModel,
            };
        }
    }
}
