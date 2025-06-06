using System;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostApprenticeshipsCSVRequest : IPostApiRequest
    {
        public long? ProviderId { get; set; }

        public string PostUrl => $"provider/{ProviderId}/apprenticeships/download";

        public object Data { get; set; }

        public PostApprenticeshipsCSVRequest(long? providerId, Body body)
        {
            ProviderId = providerId;
            Data = body;
        }

        public class Body
        {            
            public string SearchTerm { get; set; }

            public string EmployerName { get; set; }

            public string CourseName { get; set; }

            public ApprenticeshipStatus? Status { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public Alerts? Alert { get; set; }

            public ConfirmationStatus? ApprenticeConfirmationStatus { get; set; }

            public DeliveryModel? DeliveryModel { get; set; }
        }        
    }
}
