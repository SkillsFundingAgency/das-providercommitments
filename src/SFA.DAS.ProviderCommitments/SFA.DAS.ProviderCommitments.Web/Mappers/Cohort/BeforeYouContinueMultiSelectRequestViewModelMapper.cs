using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class BeforeYouContinueMultiSelectRequestViewModelMapper(IApprovalsOuterApiClient approvalsOuterApiClient) 
    : IMapper<BeforeYouContinueMultiSelectRequest, BeforeYouContinueMultiSelectViewModel>
{

    public async Task<BeforeYouContinueMultiSelectViewModel> Map(BeforeYouContinueMultiSelectRequest source)
    {
        var hasRelationshipTask = await approvalsOuterApiClient.GetHasRelationshipWithPermission(source.ProviderId);

        var result = new BeforeYouContinueMultiSelectViewModel
        {
            ProviderId = source.ProviderId,
            HasCreateCohortPermission = hasRelationshipTask.HasPermission
        };

        return result;
    }
}