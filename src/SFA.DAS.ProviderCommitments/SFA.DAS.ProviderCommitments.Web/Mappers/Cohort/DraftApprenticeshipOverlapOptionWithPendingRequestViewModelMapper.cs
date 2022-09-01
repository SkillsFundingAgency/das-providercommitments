using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

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
                OverlappingTrainingDateRequestToggleEnabled = source.OverlappingTrainingDateRequestToggleEnabled,
                Status = source.Status,
                EnableStopRequestEmail = source.EnableStopRequestEmail
            });
        }
    }
}
