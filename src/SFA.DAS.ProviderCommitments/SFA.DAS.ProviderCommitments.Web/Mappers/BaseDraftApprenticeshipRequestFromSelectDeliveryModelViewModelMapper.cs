using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class BaseDraftApprenticeshipRequestFromSelectDeliveryModelViewModelMapper : IMapper<SelectDeliveryModelViewModel, BaseDraftApprenticeshipRequest>
    {
        public Task<BaseDraftApprenticeshipRequest> Map(SelectDeliveryModelViewModel source)
        {
            return Task.FromResult(new BaseDraftApprenticeshipRequest
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId
            });
        }
    }
}