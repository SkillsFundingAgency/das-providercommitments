using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class EditDraftApprenticeshipEmailToUpdateRequestMapper : IMapper<DraftApprenticeshipAddEmailViewModel, UpdateDraftApprenticeshipApimRequest>
    {  
        public Task<UpdateDraftApprenticeshipApimRequest> Map(DraftApprenticeshipAddEmailViewModel source)
        {
            return Task.FromResult(new UpdateDraftApprenticeshipApimRequest
            {               
                Email = source.Email
            });
        }
    }
}