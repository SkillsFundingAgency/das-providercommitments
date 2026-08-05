using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class BeforeYouContinueMultiSelectRequestViewModelMapper(IApprovalsOuterApiClient approvalsOuterApiClient, IOuterApiClient apiClient) 
    : IMapper<BeforeYouContinueMultiSelectRequest, BeforeYouContinueMultiSelectViewModel>
{

    public async Task<BeforeYouContinueMultiSelectViewModel> Map(BeforeYouContinueMultiSelectRequest source)
    {
        var apiRequest = new GetConfirmEmployerRequest(source.ProviderId);
        var apiResponse = await apiClient.Get<GetConfirmEmployerResponse>(apiRequest);        

        var hasRelationshipTask = await approvalsOuterApiClient.GetHasRelationshipWithPermission(source.ProviderId);

        var result = new BeforeYouContinueMultiSelectViewModel
        {
            ProviderId = source.ProviderId,
            HasCreateCohortPermission = hasRelationshipTask.HasPermission,
            HasNoDeclaredStandards = apiResponse.HasNoDeclaredStandards
        };

        return result;
    }
}