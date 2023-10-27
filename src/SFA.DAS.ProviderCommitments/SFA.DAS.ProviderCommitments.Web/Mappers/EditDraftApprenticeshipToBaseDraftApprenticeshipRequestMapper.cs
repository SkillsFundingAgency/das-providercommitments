using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipToBaseDraftApprenticeshipRequestMapper : IMapper<EditDraftApprenticeshipViewModel, BaseDraftApprenticeshipRequest>
    {
        public Task<BaseDraftApprenticeshipRequest> Map(EditDraftApprenticeshipViewModel source) =>
            Task.FromResult(new BaseDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId
            });
    }
}
