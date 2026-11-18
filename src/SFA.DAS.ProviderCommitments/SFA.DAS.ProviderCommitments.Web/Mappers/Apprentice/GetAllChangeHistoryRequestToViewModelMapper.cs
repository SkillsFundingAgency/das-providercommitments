using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class GetAllChangeHistoryRequestToViewModelMapper(IApprovalsOuterApiClient approvalsApiClient) : IMapper<GetAllChangeHistoryRequest, GetAllChangeHistoryListViewModel>
{
    public async Task<GetAllChangeHistoryListViewModel> Map(GetAllChangeHistoryRequest source)
    {
        var pageNumber = Math.Max(source.PageNumber, 1);
        var apiRequest = new GetAllChangeHistoryApiRequest(source.ProviderId, pageNumber);
        var changeHistory = await approvalsApiClient.GetAllChangeHistory(apiRequest);

        return new GetAllChangeHistoryListViewModel
        {
            ProviderId = source.ProviderId,
            PageNumber = pageNumber,
            TotalCount = changeHistory.TotalCount,
            ChangeHistory = changeHistory.ChangeHistory.ConvertAll(x => new GetAllChangeHistoryViewModel
            {
                Description = x.Description,
                AppliedDate = x.AppliedDate,
                ChangeType = (LearningChangeType)x.ChangeType,
                Id = x.Id,
                LearnerName = x.LearnerName,
                EmployerName = x.EmployerName
            }),
            AvailableFrom = changeHistory.AvailableFrom
        };
    }
}