using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Interfaces
{
    public interface ICommitmentsOuterApiHttpClient : IRestHttpClient
    {
    }

    public class CommitmentsOuterApiClient
    {
        private readonly ICommitmentsOuterApiHttpClient _client;

        public CommitmentsOuterApiClient(ICommitmentsOuterApiHttpClient client)
        {
            _client = client;
        }

        public async Task<CourseDeliveryModels> GetCourseDeliveryModels(long providerId, string courseCode)
        {
            return await _client.Get<CourseDeliveryModels>($"providers/{providerId}/courses/{courseCode}");
        }
    }

    public class CourseDeliveryModels
    {
        public IEnumerable<DeliveryModel> DeliveryModels { get; set; }
    }
}