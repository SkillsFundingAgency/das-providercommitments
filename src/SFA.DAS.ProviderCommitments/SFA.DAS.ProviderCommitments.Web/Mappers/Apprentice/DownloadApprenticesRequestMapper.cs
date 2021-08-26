using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DownloadApprenticesRequestMapper : IMapper<DownloadRequest, DownloadViewModel>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly ICreateCsvService _createCsvService;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEncodingService _encodingService;

        public DownloadApprenticesRequestMapper(ICommitmentsApiClient client, ICreateCsvService createCsvService, ICurrentDateTime currentDateTime, IEncodingService encodingService)
        {
            _client = client;
            _createCsvService = createCsvService;
            _currentDateTime = currentDateTime;
            _encodingService = encodingService;
        }

        public async Task<DownloadViewModel> Map(DownloadRequest request)
        {
            
            var downloadViewModel = new DownloadViewModel();
            var getApprenticeshipsRequest = new GetApprenticeshipsRequest
            {
                ProviderId = request.ProviderId,
                SearchTerm = request.SearchTerm,
                EmployerName = request.SelectedEmployer,
                CourseName = request.SelectedCourse,
                Status = request.SelectedStatus,
                StartDate = request.SelectedStartDate,
                EndDate = request.SelectedEndDate,
                Alert = request.SelectedAlert,
                ApprenticeConfirmationStatus = request.SelectedApprenticeConfirmation,
                PageNumber = 0
            };

            var csvModel = new ApprenticeshipDetailsCsvModel();
            var result = await _client.GetApprenticeships(getApprenticeshipsRequest);
            var csvContent = result.Apprenticeships.Select(c => csvModel.Map(c,_encodingService)).ToList();

            downloadViewModel.Content = _createCsvService.GenerateCsvContent(csvContent, true);
            downloadViewModel.Request = getApprenticeshipsRequest;
            downloadViewModel.Name = $"{"Manageyourapprentices"}_{_currentDateTime.UtcNow:yyyyMMddhhmmss}.csv";
            return await Task.FromResult(downloadViewModel);
        }

    }
}
