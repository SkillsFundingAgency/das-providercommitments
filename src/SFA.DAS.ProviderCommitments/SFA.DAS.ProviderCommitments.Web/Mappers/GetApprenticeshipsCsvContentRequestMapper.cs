using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class GetApprenticeshipsCsvContentRequestMapper : IMapper<GetApprenticeshipsCsvContentRequest,byte[]>
    {
        private readonly ICommitmentsApiClient _client;
        private readonly ICreateCsvService _createCsvService;

        public GetApprenticeshipsCsvContentRequestMapper(ICommitmentsApiClient client, ICreateCsvService createCsvService)
        {
            _client = client;
            _createCsvService = createCsvService;
        }

        public async Task<byte[]> Map(GetApprenticeshipsCsvContentRequest request)
        {
            var response = await _client.GetApprenticeships(new GetApprenticeshipsRequest{ProviderId = request.ProviderId});

            var csvContent = response.Apprenticeships.Select(c => (ApprenticeshipDetailsCsvViewModel)c).ToList();
            
            var csvFileContent = _createCsvService.GenerateCsvContent(csvContent);

            return csvFileContent;
        }
    }
}
