using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface IApprovalsOuterApiClient
    {
        Task<ProviderCourseDeliveryModels> GetProviderCourseDeliveryModels(
            long providerId,
            string courseCode,
            long accountLegalEntityId,
            CancellationToken cancellationToken = default);
    }
}