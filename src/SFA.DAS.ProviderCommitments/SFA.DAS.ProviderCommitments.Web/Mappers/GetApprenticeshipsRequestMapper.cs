using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models;
using GetApprenticeshipsRequest = SFA.DAS.CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class GetApprenticeshipsRequestMapper : IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>
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
            var response = await _client.GetApprenticeships(new GetApprenticeshipsRequest
            {
                ProviderId = source.ProviderId, 
                PageNumber = source.PageNumber, 
                PageItemCount = source.PageItemCount
            });
            
            var filterModel = new ManageApprenticesFilterModel
            {
                TotalNumberOfApprenticeshipsFound = response.TotalApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = response.TotalApprenticeshipsWithAlertsFound,
                PageNumber = source.PageNumber
            };

            var apprenticeshipTasks = response.Apprenticeships.Select(c => _mapper.Map(c)).ToArray();
            Task.WaitAll(apprenticeshipTasks.Cast<Task>().ToArray());

            return new ManageApprenticesViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships =apprenticeshipTasks.Select(t => t.Result),
                FilterModel = filterModel
            };
        }
    }
}
