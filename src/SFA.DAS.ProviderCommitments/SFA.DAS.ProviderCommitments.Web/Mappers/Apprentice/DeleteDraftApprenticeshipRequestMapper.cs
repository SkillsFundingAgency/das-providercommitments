using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DeleteDraftApprenticeshipRequestMapper : IMapper<DeleteConfirmationViewModel, DeleteDraftApprenticeshipRequest>
    {
        public Task<DeleteDraftApprenticeshipRequest> Map(DeleteConfirmationViewModel source)
        {   
            return Task.FromResult(new DeleteDraftApprenticeshipRequest());
        }
    }
}
