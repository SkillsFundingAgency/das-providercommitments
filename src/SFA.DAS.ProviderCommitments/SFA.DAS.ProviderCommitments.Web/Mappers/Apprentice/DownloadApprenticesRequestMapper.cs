using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DownloadApprenticesRequestMapper : IMapper<DownloadRequest, DownloadViewModel>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly ICreateCsvService _createCsvService;
        private readonly ICurrentDateTime _currentDateTime;

        public DownloadApprenticesRequestMapper(ICommitmentsApiClient client, ICreateCsvService createCsvService, ICurrentDateTime currentDateTime)
        {
            _client = client;
            _createCsvService = createCsvService;
            _currentDateTime = currentDateTime;
        }

        public async Task<DownloadViewModel> Map(DownloadRequest request)
        {
            var downloadViewModel = new DownloadViewModel();
            var response = await _client.GetApprenticeships(new GetApprenticeshipsRequest
            {
                ProviderId = request.ProviderId,
                SearchTerm = request.SearchTerm,
                EmployerName = request.SelectedEmployer,
                CourseName = request.SelectedCourse,
                Status = request.SelectedStatus,
                StartDate = request.SelectedStartDate,
                EndDate = request.SelectedEndDate,
                PageNumber = 0
            });

            var csvContent = response.Apprenticeships.Select(c => (ApprenticeshipDetailsCsvModel)c).ToList();

            downloadViewModel.Content = _createCsvService.GenerateCsvContent(csvContent, true).ToArray();
            downloadViewModel.Name = $"{"Manageyourapprentices"}_{_currentDateTime.UtcNow:yyyyMMddhhmmss}.csv";
            return downloadViewModel;
        }
    }
}
