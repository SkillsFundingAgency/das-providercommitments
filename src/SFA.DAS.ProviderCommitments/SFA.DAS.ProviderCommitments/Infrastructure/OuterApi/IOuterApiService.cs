using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IOuterApiService
    {
        Task<GetAccountLegalEntityQueryResult> GetAccountLegalEntity(long publicAccountLegalEntityId);
        Task<GetDraftApprenticeshipsResult> GetDraftApprenticeships(long cohortId);
        Task<GetCohortResult> GetCohort(long cohortId);
        Task<GetStandardResponse> GetStandardDetails(string courseCode);
        Task ValidateBulkUploadRequest(BulkUploadValidateApiRequest data);
    }
}
