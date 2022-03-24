using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IOuterApiService
    {
        Task<GetAccountLegalEntityQueryResult> GetAccountLegalEntity(long publicAccountLegalEntityId);
        Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId);
    }
}
