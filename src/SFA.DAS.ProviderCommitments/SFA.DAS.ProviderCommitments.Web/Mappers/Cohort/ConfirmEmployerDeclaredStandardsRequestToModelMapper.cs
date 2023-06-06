using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ConfirmEmployerDeclaredStandardsRequestToModelMapper : IMapper<ConfirmEmployerViewModel, ConfirmEmployerRedirectModel>
    {
        private readonly IOuterApiClient _apiClient;

        public ConfirmEmployerDeclaredStandardsRequestToModelMapper(IOuterApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ConfirmEmployerRedirectModel> Map(ConfirmEmployerViewModel source)
        {
            var apiRequest = new GetConfirmEmployerDeclaredStandardsRequest(source.ProviderId);
            var apiResponse = await _apiClient.Get<GetConfirmEmployerDeclaredStandardsResponse>(apiRequest);

            return new ConfirmEmployerRedirectModel
            {
                HasNoDeclaredStandards = apiResponse.HasNoDeclaredStandards
            };
        }
    }
}
