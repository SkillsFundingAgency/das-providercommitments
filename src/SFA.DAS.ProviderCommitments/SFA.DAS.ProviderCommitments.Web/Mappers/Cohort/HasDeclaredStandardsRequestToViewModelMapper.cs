using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class HasDeclaredStandardsRequestToViewModelMapper : IMapper<ConfirmEmployerViewModel, HasDeclaredStandardsViewModel>
    {
        private readonly IOuterApiClient _apiClient;

        public HasDeclaredStandardsRequestToViewModelMapper(IOuterApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<HasDeclaredStandardsViewModel> Map(ConfirmEmployerViewModel source)
        {
            var apiRequest = new GetHasDeclaredStandardsRequest(source.ProviderId);
            var apiResponse = await _apiClient.Get<GetHasDeclaredStandardsResponse>(apiRequest);

            return new HasDeclaredStandardsViewModel
            {
                HasNoDeclaredStandards = apiResponse.HasNoDeclaredStandards
            };
        }
    }
}
