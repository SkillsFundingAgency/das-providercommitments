using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface ICommitmentsService
    {
        Task<CohortDetails> GetCohortDetail(long cohortId);
        Task AddDraftApprenticeshipToCohort(long cohortId, AddDraftApprenticeshipRequest request);
        Task<EditDraftApprenticeshipDetails> GetDraftApprenticeshipForCohort(long cohortId, long draftApprenticeshipId);
        Task UpdateDraftApprenticeship(long cohortId, long draftApprenticeshipId, UpdateDraftApprenticeshipRequest updateRequest);
    }
}