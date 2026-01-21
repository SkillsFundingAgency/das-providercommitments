using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipToDraftApprenticeshipRequestMapper : IMapper<EditDraftApprenticeshipViewModel, DraftApprenticeshipRequest>
    {
        public Task<DraftApprenticeshipRequest> Map(EditDraftApprenticeshipViewModel source)
        {
            if (!source.CohortId.HasValue)
            {
                throw new InvalidOperationException("CohortId is required when editing a draft apprenticeship.");
            }

            if (!source.DraftApprenticeshipId.HasValue)
            {
                throw new InvalidOperationException("DraftApprenticeshipId is required when editing a draft apprenticeship.");
            }

            return Task.FromResult(new DraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                CohortId = source.CohortId.Value,
                DraftApprenticeshipId = source.DraftApprenticeshipId.Value
            });
        }
    }
}
