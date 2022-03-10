using SFA.DAS.ProviderCommitments.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Infrastructure
{
    public class CommitmentsOuterApiClient
    {
        private readonly ICommitmentsOuterApiHttpClient _client;

        public CommitmentsOuterApiClient(ICommitmentsOuterApiHttpClient client)
        {
            _client = client;
        }

        public async Task<CourseDeliveryModels> GetCourseDeliveryModels(
            long providerId,
            string courseCode,
            CancellationToken cancellationToken = default)
        {
            return await _client.Get<CourseDeliveryModels>
                ($"providers/{providerId}/courses/{courseCode}",
                cancellationToken: cancellationToken);
        }
    }
}