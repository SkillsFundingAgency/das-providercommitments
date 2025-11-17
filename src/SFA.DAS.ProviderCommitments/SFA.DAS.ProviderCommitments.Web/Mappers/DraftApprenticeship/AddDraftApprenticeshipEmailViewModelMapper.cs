using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship
{
    public class AddDraftApprenticeshipEmailViewModelMapper
    : IMapper<EmailRequest, AddDraftApprenticeshipEmailViewModel>
    {
        public async Task<AddDraftApprenticeshipEmailViewModel> Map(EmailRequest source)
        {
            var result = new AddDraftApprenticeshipEmailViewModel()
            {
                ProviderId = source.ProviderId,
                CohortId = source.CohortId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                Name = source.Name,
                Email = source.Email,
                CohortReference = source.CohortReference,
            };

            return result;
        }
    }
}
