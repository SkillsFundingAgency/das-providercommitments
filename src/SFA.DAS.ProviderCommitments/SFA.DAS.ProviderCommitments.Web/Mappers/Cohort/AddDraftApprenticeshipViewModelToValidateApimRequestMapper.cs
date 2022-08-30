using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class AddDraftApprenticeshipViewModelToValidateApimRequestMapper : IMapper<AddDraftApprenticeshipViewModel, ValidateDraftApprenticeshipApimRequest>
    {
        public Task<ValidateDraftApprenticeshipApimRequest> Map(AddDraftApprenticeshipViewModel source)
        {
            var result = source.MapDraftApprenticeship();
            return Task.FromResult(result);
        }
    }
}
