using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class GetApprenticeshipsRequestMapper : IMapper<GetApprenticeshipsRequest,ManageApprenticesViewModel>
    {
        private readonly ICommitmentsApiClient _client;

        public GetApprenticeshipsRequestMapper(ICommitmentsApiClient client)
        {
            _client = client;
        }

        public async Task<ManageApprenticesViewModel> Map(GetApprenticeshipsRequest source)
        {
            var response = await _client.GetApprenticeships(new GetApprenticeshipRequest
            {
                ProviderId = source.ProviderId, 
                PageNumber = source.PageNumber, 
                PageItemCount = source.PageItemCount,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort
            });
            
            var filterModel = new ManageApprenticesFilterModel
            {
                TotalNumberOfApprenticeshipsFound = response.TotalApprenticeshipsFound,
                TotalNumberOfApprenticeshipsWithAlertsFound = response.TotalApprenticeshipsWithAlertsFound,
                PageNumber = source.PageNumber
            };

            return new ManageApprenticesViewModel
            {
                ProviderId = source.ProviderId,
                Apprenticeships = response.Apprenticeships,
                FilterModel = filterModel,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort
            };
        }
    }
}
