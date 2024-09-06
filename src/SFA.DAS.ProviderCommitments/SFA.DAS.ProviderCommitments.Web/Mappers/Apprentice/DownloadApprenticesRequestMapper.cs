using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DownloadApprenticesRequestMapper : IMapper<DownloadRequest, DownloadViewModel>
    {
        private readonly IOuterApiService _outerApiService;
        private readonly ICreateCsvService _createCsvService;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEncodingService _encodingService;

        public DownloadApprenticesRequestMapper(IOuterApiService outerApiService, ICreateCsvService createCsvService, ICurrentDateTime currentDateTime, IEncodingService encodingService)
        {
            _outerApiService = outerApiService;
            _createCsvService = createCsvService;
            _currentDateTime = currentDateTime;
            _encodingService = encodingService;
        }

        public async Task<DownloadViewModel> Map(DownloadRequest request)
        {

            var downloadViewModel = new DownloadViewModel();

            var apiRequestBody = new PostApprenticeshipsCSVRequest.Body
            {
                SearchTerm = request.SearchTerm,
                EmployerName = request.SelectedEmployer,
                CourseName = request.SelectedCourse,
                Status = request.SelectedStatus,
                StartDate = request.SelectedStartDate,
                EndDate = request.SelectedEndDate,
                Alert = request.SelectedAlert,
                ApprenticeConfirmationStatus = request.SelectedApprenticeConfirmation,
                DeliveryModel = request.SelectedDeliveryModel
            };

            var getApprenticeshipsRequest = new PostApprenticeshipsCSVRequest(
               providerId: request.ProviderId,
              apiRequestBody
           );

            var csvModel = new ApprenticeshipDetailsCsvModel();
            var result = await _outerApiService.GetApprenticeshipsCSV(getApprenticeshipsRequest);
            var csvContent = result.Apprenticeships.Select(c => csvModel.Map(c, _encodingService)).ToList();

            downloadViewModel.Content = _createCsvService.GenerateCsvContent(csvContent, true);
            downloadViewModel.Request = getApprenticeshipsRequest;
            downloadViewModel.Name = $"{"Manageyourapprentices"}_{_currentDateTime.UtcNow:yyyyMMddhhmmss}.csv";
            return downloadViewModel;
        }
    }
}
