using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class LearnerDataSyncMapper(IOuterApiService outerApiService, ICacheStorageService cacheStorageService)
    : IMapper<EditDraftApprenticeshipViewModel, LearnerDataSyncResult>
{
    public async Task<LearnerDataSyncResult> Map(EditDraftApprenticeshipViewModel model)
    {
        try
        {
            var response = await outerApiService.SyncLearnerData(model.ProviderId, model.CohortId.Value, model.DraftApprenticeshipId.Value);
                
            if (response.Success)
            {
                var cacheKey = Guid.NewGuid();
                await cacheStorageService.SaveToCache(cacheKey, response.UpdatedDraftApprenticeship, expirationInHours: 1);
                    
                return new LearnerDataSyncResult
                {
                    Success = true,
                    CacheKey = cacheKey.ToString(),
                    Message = "Learner data has been successfully updated."
                };
            }

            return new LearnerDataSyncResult
            {
                Success = false,
                Message = response.Message ?? "Failed to sync learner data."
            };
        }
        catch (Exception)
        {
            return new LearnerDataSyncResult
            {
                Success = false,
                Message = "An error occurred while syncing learner data."
            };
        }
    }
}

public class LearnerDataSyncResult
{
    public bool Success { get; set; }
    public string CacheKey { get; set; }
    public string Message { get; set; } = string.Empty;
}