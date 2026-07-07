using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class GetAllChangeHistoryRequestToViewModelMapper(IApprovalsOuterApiClient approvalsApiClient) : IMapper<GetAllChangeHistoryRequest, GetAllChangeHistoryListViewModel>
{
    public async Task<GetAllChangeHistoryListViewModel> Map(GetAllChangeHistoryRequest source)
    {
        var changeHistory = await approvalsApiClient.GetAllChangeHistory(source.ProviderId);

        return new GetAllChangeHistoryListViewModel
        {
            ProviderId = source.ProviderId,
            ChangeHistory = changeHistory.ChangeHistory.ConvertAll(x => new GetAllChangeHistoryViewModel
            {
                Description = x.Description,
                AppliedDate = x.AppliedDate,
                ChangeType = (LearningChangeType)x.ChangeType,
                Id = x.Id,
                LearnerName = x.LearnerName,
                EmployerName = x.EmployerName
            }),
            AvailableFrom = changeHistory.ChangeHistory.OrderBy(t => t.Created).FirstOrDefault()?.Created ?? DateTime.UtcNow
        };
    }
}