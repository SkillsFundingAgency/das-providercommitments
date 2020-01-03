using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.Commitments.Shared.Interfaces
{
    public interface ICommitmentsService
    {
        Task AddDraftApprenticeshipToCohort(long cohortId, AddDraftApprenticeshipRequest request);
        Task<CreateCohortResponse> CreateCohort(CreateCohortRequest request);
        Task<CohortDetails> GetCohortDetail(long cohortId);
        Task<EditDraftApprenticeshipDetails> GetDraftApprenticeshipForCohort(long cohortId, long draftApprenticeshipId);
        Task UpdateDraftApprenticeship(long cohortId, long draftApprenticeshipId, UpdateDraftApprenticeshipRequest updateRequest);
        Task<GetApprenticeshipsFilteredResult> GetApprenticeships(uint providerId, uint pageNumber);
    }
}