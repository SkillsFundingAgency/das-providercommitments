using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class DraftApprenticeshipOverlapOptionWithPendingRequestViewModelMapper : IMapper<DraftApprenticeshipOverlapOptionWithPendingRequestViewModel, DraftApprenticeshipOverlapOptionViewModel>
    {
        public Task<DraftApprenticeshipOverlapOptionViewModel> Map(DraftApprenticeshipOverlapOptionWithPendingRequestViewModel source)
        {
            return Task.FromResult(new DraftApprenticeshipOverlapOptionViewModel
            {
                DraftApprenticeshipId = source.DraftApprenticeshipId.Value,
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                OverlapOptions = source.OverlapOptions,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                Status = source.Status,
                EnableStopRequestEmail = source.EnableStopRequestEmail
            });
        }
    }
}
