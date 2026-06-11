using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class ChangeHistoryRequestToViewModelMapper(IApprovalsOuterApiClient approvalsApiClient) : IMapper<ChangeHistoryRequest, ChangeHistoryListViewModel>
{
    public async Task<ChangeHistoryListViewModel> Map(ChangeHistoryRequest source)
    {
        var changeHistory = await approvalsApiClient.GetChangeHistory(source.ApprenticeshipId);

        return new ChangeHistoryListViewModel
        {
            ApprenticeshipHashedId = source.ApprenticeshipHashedId,
            ProviderId = source.ProviderId,
            ChangeHistory = changeHistory.ChangeHistory.ConvertAll(x => new ChangeHistoryViewModel
            {
                Description = x.Description,
                AppliedDate = x.AppliedDate,
                ChangeType = (LearningChangeType)x.ChangeType,
                Id = x.Id
            }),
            Name = changeHistory.ChangeHistory.FirstOrDefault()?.LearnerName,
            AvailableFrom = changeHistory.ChangeHistory.OrderBy(t => t.Created).FirstOrDefault()?.Created ?? DateTime.UtcNow
        };
    }
}