using SFA.DAS.CommitmentsV2.Types;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Queries.GetProviderCourseDeliveryModels
{
    public class GetProviderCourseDeliveryModelsQueryResponse
    {
        public IEnumerable<DeliveryModel> DeliveryModels { get; set; }
    }
}