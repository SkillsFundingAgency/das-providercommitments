using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class ChangeHistoryRequestToViewModelMapper(IApprovalsOuterApiClient approvalsApiClient,
        IEncodingService encodingService) : IMapper<ChangeHistoryRequest, ChangeHistoryListViewModel>
{
    public async Task<ChangeHistoryListViewModel> Map(ChangeHistoryRequest source)
    {
        var apprenticeshipId = encodingService.Decode(source.ApprenticeshipHashedId, EncodingType.ApprenticeshipId);
        var changeHistory = await approvalsApiClient.GetChangeHistory(apprenticeshipId);

        return new ChangeHistoryListViewModel
        {
            ApprenticeshipHashedId = source.ApprenticeshipHashedId,
            ProviderId = source.ProviderId,
            ChangeHistory = [.. changeHistory.ChangeHistory.Select(x => new ChangeHistoryViewModel
             {
                 Description = x.Description,
                 AppliedDate = x.AppliedDate,
                 ChangeType = (LearningChangeType)x.ChangeType,
                 Id = x.Id
             })],
            Name = changeHistory.ChangeHistory.FirstOrDefault().LearnerName
        };
    }
}