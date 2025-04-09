using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectIlrRecordViewModelMapper : IMapper<SelectIlrRecordRequest, SelectIlrRecordViewModel>
    {
        private readonly IApprovalsOuterApiClient _client;
        private readonly IModelMapper _modelMapper;

        public SelectIlrRecordViewModelMapper(IApprovalsOuterApiClient client, IModelMapper modelMapper)
        {
            _client = client;
            _modelMapper = modelMapper;
        }

        public async Task<SelectIlrRecordViewModel> Map(SelectIlrRecordRequest source)
        {
            //var response = await _client.GetIlrRecords(new GetApprenticeshipsRequest
            //{
            //    ProviderId = source.ProviderId,
            //    PageNumber = source.PageNumber,
            //    PageItemCount = Constants.ApprenticesSearch.NumberOfApprenticesPerSearchPage,
            //    SortField = source.SortField,
            //    ReverseSort = source.ReverseSort,
            //    SearchTerm = source.SearchTerm,
            //    EmployerName = source.SelectedEmployer,
            //    CourseName = source.SelectedCourse,
            //    Status = source.SelectedStatus,
            //    StartDate = source.SelectedStartDate,
            //    EndDate = source.SelectedEndDate,
            //    Alert = source.SelectedAlert,
            //    ApprenticeConfirmationStatus = source.SelectedApprenticeConfirmation,
            //    DeliveryModel = source.SelectedDeliveryModel,
            //    IsOnFlexiPaymentPilot = source.SelectedPilotStatus
            //});



            var filterModel = new IlrRecordsFilterModel()
            {
                ProviderId = source.ProviderId,
                TotalNumberOfApprenticeships = 1000, //response.TotalApprenticeships,
                TotalNumberOfApprenticeshipsFound = 100, //response.TotalApprenticeshipsFound,
                PageNumber = 1, //response.PageNumber,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
            };

            //if (response.TotalApprenticeships >= Constants.ApprenticesSearch.NumberOfApprenticesRequiredForSearch)
            //{
            //    var filters = await _client.GetApprenticeshipsFilterValues(
            //        new GetApprenticeshipFiltersRequest{ProviderId = source.ProviderId});

            //    filterModel.EmployerFilters = filters.EmployerNames;
            //    filterModel.CourseFilters = filters.CourseNames;
            //    filterModel.StartDateFilters = filters.StartDates;
            //    filterModel.EndDateFilters = filters.EndDates;
            //}

            //var apprenticeships = new List<ApprenticeshipDetailsViewModel>();
            //foreach (var apprenticeshipDetailsResponse in response.Apprenticeships)
            //{
            //    var apprenticeship = await _modelMapper.Map<ApprenticeshipDetailsViewModel>(apprenticeshipDetailsResponse);
            //    apprenticeships.Add(apprenticeship);
            //}


            var model= new SelectIlrRecordViewModel
            {
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                EmployerAccountName = "Test needs to be set",
                FilterModel = filterModel
            };
            model.SortedByHeader();
            return model;
        }
    }
}
