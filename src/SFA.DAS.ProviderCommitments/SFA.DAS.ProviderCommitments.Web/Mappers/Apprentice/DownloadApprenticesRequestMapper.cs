using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DownloadApprenticesRequestMapper : IMapper<DownloadRequest, byte[]>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly ICreateCsvService _createCsvService;

        public DownloadApprenticesRequestMapper(ICommitmentsApiClient client, ICreateCsvService createCsvService)
        {
            _client = client;
            _createCsvService = createCsvService;
        }

        public async Task<byte[]> Map(DownloadRequest request)
        {
            var response = await _client.GetApprenticeships(new GetApprenticeshipsRequest
            {
                ProviderId = request.ProviderId,
                SearchTerm = request.SearchTerm,
                EmployerName = request.SelectedEmployer,
                CourseName = request.SelectedCourse,
                Status = request.SelectedStatus,
                StartDate = request.SelectedStartDate,
                EndDate = request.SelectedEndDate
            });

            var csvContent = response.Apprenticeships.Select(c => (ApprenticeshipDetailsCsvModel)c).ToList();
            
            var csvFileContent = _createCsvService.GenerateCsvContent(csvContent);

            return csvFileContent;
        }
    }
}
