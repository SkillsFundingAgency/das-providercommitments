using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Models.ApiModels;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IProviderCommitmentsService
    {
        Task<CohortDetails> GetCohortDetail(long cohortId);
        Task AddDraftApprenticeshipToCohort(AddDraftApprenticeshipToCohortRequest request);
    }
}