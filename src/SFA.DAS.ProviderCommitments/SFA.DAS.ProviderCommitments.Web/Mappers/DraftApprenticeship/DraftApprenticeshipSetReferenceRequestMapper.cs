using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class DraftApprenticeshipSetReferenceRequestMapper : IMapper<EditDraftApprenticeshipViewModel, DraftApprenticeshipSetReferenceRequest>
{
    public Task<DraftApprenticeshipSetReferenceRequest> Map(EditDraftApprenticeshipViewModel source)
    {
        var endDay = source.EndDay < 10 ? $"0{source.EndDay}" : source.EndDay.ToString();

        return Task.FromResult(new DraftApprenticeshipSetReferenceRequest
        {
            Reference = source.Email,
            CohortId = source.CohortId.GetValueOrDefault(),
            DraftApprenticeshipId = source.DraftApprenticeshipId.Value,
            Name = $"{source.FirstName} {source.LastName}",
            Party = CommitmentsV2.Types.Party.Provider,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
            ProviderId = source.ProviderId
        });
    }
}