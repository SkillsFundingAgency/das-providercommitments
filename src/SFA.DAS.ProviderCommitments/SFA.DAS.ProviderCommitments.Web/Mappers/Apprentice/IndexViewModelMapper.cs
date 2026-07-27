using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class IndexViewModelMapper : IMapper<IndexRequest, IndexViewModel>
{
    private readonly IModelMapper _modelMapper;
    private readonly IApprovalsOuterApiClient _approvalsOuterApiClient;
    private readonly ILogger<IndexViewModelMapper> _logger;

    public IndexViewModelMapper(IModelMapper modelMapper, IApprovalsOuterApiClient approvalsOuterApiClient, ILogger<IndexViewModelMapper> logger)
    {
        _modelMapper = modelMapper;
        _approvalsOuterApiClient = approvalsOuterApiClient;
        _logger = logger;
    }

    public async Task<IndexViewModel> Map(IndexRequest source)
    {
        try
        {
            _logger.LogInformation("Inside Index view model mapper");

            var response = await _approvalsOuterApiClient.GetApprenticeships(new
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

            _logger.LogInformation("Response is null {a}", response is null);

            response = response ?? new Infrastructure.OuterApi.Responses.Apprentices.GetApprenticeshipsResponse();

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

            _logger.LogInformation("Processed successfully");

            return new IndexViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships = apprenticeships,
                FilterModel = filterModel,
                HasChangeHistory = response.HasChangeHistory
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        return new IndexViewModel();
    }
}