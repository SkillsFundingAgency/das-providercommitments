using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.ProviderCommitments.Models;
using SFA.DAS.ProviderCommitments.Models.ApiModels;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IProviderCommitmentsService
    {
        Task<CohortDetails> GetCohortDetail(long cohortId);
        Task AddDraftApprenticeshipToCohort(AddDraftApprenticeshipToCohortRequest request);
        Task<EditDraftApprenticeshipDetails> GetDraftApprenticeshipForCohort(long cohortId, long draftApprenticeshipId);
        Task UpdateDraftApprenticeship(long cohortId, long draftApprenticeshipId, UpdateDraftApprenticeshipRequest updateRequest);
    }
}