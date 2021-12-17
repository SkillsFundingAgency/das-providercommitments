using Microsoft.AspNetCore.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public interface IBulkUploadFileParser
    {
        BulkUploadAddDraftApprenticeshipsRequest CreateApiRequest(long providerId, IFormFile attachment);
    }
}
