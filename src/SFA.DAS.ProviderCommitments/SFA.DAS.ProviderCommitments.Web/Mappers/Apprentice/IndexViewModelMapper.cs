using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class IndexViewModelMapper : IMapper<IndexRequest, IndexViewModel>
{
    private readonly IModelMapper _modelMapper;
    private readonly IOuterApiService _outerApiService;

    public IndexViewModelMapper(IModelMapper modelMapper, IOuterApiService outerApiService)
    {
        _modelMapper = modelMapper;
        _outerApiService = outerApiService;
    }

    public async Task<IndexViewModel> Map(IndexRequest source)
    {
        var response = await _outerApiService.GetApprenticeships(new
            GetApprenticeshipsRequest(
            source.ProviderId,
            source.PageNumber,
            Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage,
            source.SortField,
            source.ReverseSort,
            source.SearchTerm,
            source.SelectedEmployer, null,
            source.SelectedCourse,
            source.SelectedStatus,
            source.SelectedStartDate,
            source.SelectedEndDate, null, null, null,
            source.SelectedAlert,
            source.SelectedApprenticeConfirmation,
            source.SelectedDeliveryModel));

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
            Alerts.ChangesForReview ,
            Alerts.ChangesPending,
            Alerts.ChangesRequested,
            Alerts.IlrDataMismatch,
        };

        var filterModel = new ApprenticesFilterModel
        {
            ProviderId = source.ProviderId,
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
            SelectedAlert = source.SelectedAlert,
            SelectedApprenticeConfirmation = source.SelectedApprenticeConfirmation,
            SelectedDeliveryModel = source.SelectedDeliveryModel,
            StatusFilters = statusFilters,
            AlertFilters = alertFilters
        };

        if (response.TotalApprenticeships >= Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch)
        {
            filterModel.EmployerFilters = response.ApprenticeshipFiltersValue.EmployerNames;
            filterModel.CourseFilters = response.ApprenticeshipFiltersValue.CourseNames;
            filterModel.StartDateFilters = response.ApprenticeshipFiltersValue.StartDates;
            filterModel.EndDateFilters = response.ApprenticeshipFiltersValue.EndDates;
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
            HasChangeHistory = response.HasChangeHistory
        };
    }
}