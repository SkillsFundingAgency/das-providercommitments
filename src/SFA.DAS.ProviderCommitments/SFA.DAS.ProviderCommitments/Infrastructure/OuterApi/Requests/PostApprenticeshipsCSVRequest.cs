using System;
using System.Collections.Generic;
using System.Net;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests
{
    public class PostApprenticeshipsCSVRequest : IPostApiRequest
    {
        public long? ProviderId { get; set; }

        public string FilterQuery { get; set; }
        public string PostUrl => $"provider/{ProviderId}/apprenticeships/download";

        public object Data { get; set; }

        public PostApprenticeshipsCSVRequest(long? providerId, Body body)
        {
            ProviderId = providerId;
            Data = body;
            FilterQuery = CreateFilterQuery(body);
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
     
        private static string CreateFilterQuery(Body request)
        {
            var queryParameters = new List<string>();

            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryParameters.Add($"searchTerm={WebUtility.UrlEncode(request.SearchTerm)}");

            if (!string.IsNullOrEmpty(request.EmployerName))
                queryParameters.Add($"employerName={WebUtility.UrlEncode(request.EmployerName)}");

            if (!string.IsNullOrEmpty(request.CourseName))
                queryParameters.Add($"courseName={WebUtility.UrlEncode(request.CourseName)}");

            if (request.Status.HasValue)
                queryParameters.Add($"status={WebUtility.UrlEncode(request.Status.Value.ToString())}");

            if (request.StartDate.HasValue)
                queryParameters.Add($"startDate={WebUtility.UrlEncode(request.StartDate.Value.ToString("u"))}");

            if (request.EndDate.HasValue)
                queryParameters.Add($"endDate={WebUtility.UrlEncode(request.EndDate.Value.ToString("u"))}");

            if (request.Alert.HasValue)
                queryParameters.Add($"alert={WebUtility.UrlEncode(request.Alert.Value.ToString())}");

            if (request.ApprenticeConfirmationStatus.HasValue)
                queryParameters.Add($"apprenticeConfirmationStatus={WebUtility.UrlEncode(request.ApprenticeConfirmationStatus.ToString())}");

            if (request.DeliveryModel.HasValue)
                queryParameters.Add($"deliveryModel={WebUtility.UrlEncode(request.DeliveryModel.ToString())}");

            return queryParameters.Any() ? "&" + string.Join("&", queryParameters) : string.Empty;
        }
    }
}
