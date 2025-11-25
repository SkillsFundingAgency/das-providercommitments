using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;
public class EditDraftApprenticeToEmailRequestMapper : IMapper<EditDraftApprenticeshipViewModel, DraftApprenticeshipAddEmailRequest>
{
    public Task<DraftApprenticeshipAddEmailRequest> Map(EditDraftApprenticeshipViewModel source)
    {
        var endDay = source.EndDay < 10 ? $"0{source.EndDay}" : source.EndDay.ToString();

        return Task.FromResult(new DraftApprenticeshipAddEmailRequest
        {
            Email = source.Email,
            CohortId = source.CohortId.GetValueOrDefault(),
            CohortReference = source.CohortReference,
            ProviderId = source.ProviderId,
            DraftApprenticeshipId = source.DraftApprenticeshipId.Value,
            DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
            Name = $"{source.FirstName} {source.LastName}",
            StartDate = $"01/{source.StartMonth}/{source.StartYear}",
            EndDate = $"{endDay}/{source.EndMonth}/{source.EndYear}",
        });
    }
}
