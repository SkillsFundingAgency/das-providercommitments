using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class EditDraftApprenticeshipViewModelToValidateApimRequestMapper : IMapper<EditDraftApprenticeshipViewModel, ValidateDraftApprenticeshipApimRequest>
    {
        public Task<ValidateDraftApprenticeshipApimRequest> Map(EditDraftApprenticeshipViewModel source)
        {
            var result = source.MapDraftApprenticeship();
            result.Id = source.DraftApprenticeshipId.Value;
            return Task.FromResult(result);
        }
    }
}
