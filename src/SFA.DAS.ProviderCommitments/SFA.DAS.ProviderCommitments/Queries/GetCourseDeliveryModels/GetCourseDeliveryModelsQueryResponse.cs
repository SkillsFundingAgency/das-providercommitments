using SFA.DAS.CommitmentsV2.Types;
using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Queries.GetCourseDeliveryModels
{
    public class GetCourseDeliveryModelsQueryResponse
    {
        public IEnumerable<DeliveryModel> DeliveryModels { get; set; }
    }
}