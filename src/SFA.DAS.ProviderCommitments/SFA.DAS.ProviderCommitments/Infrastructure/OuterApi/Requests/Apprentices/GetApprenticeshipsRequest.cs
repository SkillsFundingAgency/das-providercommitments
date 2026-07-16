using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

public class GetApprenticeshipsRequest(long? providerId, int pageNumber, int pageItemCount, string sortField, bool reverseSort, string searchTerm, string employerName, string providerName, string courseName, ApprenticeshipStatus? status, DateTime? startDate, DateTime? endDate, int? accountLegalEntityId, DateTime? startDateRangeFrom, DateTime? startDateRangeTo, Alerts? alert, ConfirmationStatus? apprenticeConfirmationStatus, DeliveryModel? deliveryModel) : IGetApiRequest
{
    public long? AccountId { get; set; }

    public long? ProviderId { get; set; } = providerId;

    public int PageNumber { get; set; } = pageNumber;

    public int PageItemCount { get; set; } = pageItemCount;

    public string SortField { get; set; } = sortField;

    public bool ReverseSort { get; set; } = reverseSort;

    public string SearchTerm { get; set; } = searchTerm;
    public string EmployerName { get; set; } = employerName;

    public string ProviderName { get; set; } = providerName;

    public string CourseName { get; set; } = courseName;

    public ApprenticeshipStatus? Status { get; set; } = status;

    public DateTime? StartDate { get; set; } = startDate;

    public DateTime? EndDate { get; set; } = endDate;
    public int? AccountLegalEntityId { get; set; } = accountLegalEntityId;

    public DateTime? StartDateRangeFrom { get; set; } = startDateRangeFrom;

    public DateTime? StartDateRangeTo { get; set; } = startDateRangeTo;

    public Alerts? Alert { get; set; } = alert;

    public ConfirmationStatus? ApprenticeConfirmationStatus { get; set; } = apprenticeConfirmationStatus;
    public DeliveryModel? DeliveryModel { get; set; } = deliveryModel;

    public string GetUrl
    {
        get
        {
            var queryParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(SearchTerm))
                queryParameters.Add("searchTerm", SearchTerm);

            if (!string.IsNullOrEmpty(EmployerName))
                queryParameters.Add("employerName", EmployerName);

            if (PageNumber > 0)
                queryParameters.Add("pageNumber", PageNumber.ToString());

            if (PageItemCount > 0)
                queryParameters.Add("pageItemCount", PageItemCount.ToString());

            if (!string.IsNullOrEmpty(SortField))
                queryParameters.Add("sortField", SortField);

            if (ReverseSort)
                queryParameters.Add("reverseSort", ReverseSort.ToString().ToLower());

            if (!string.IsNullOrEmpty(CourseName))
                queryParameters.Add("courseName", CourseName);

            if (!string.IsNullOrEmpty(ProviderName))
                queryParameters.Add("providerName", ProviderName);

            if (Status.HasValue)
                queryParameters.Add("status", Status.Value.ToString());

            if (StartDate.HasValue)
                queryParameters.Add("startDate", StartDate.Value.ToString("u"));

            if (EndDate.HasValue)
                queryParameters.Add("endDate", EndDate.Value.ToString("u"));

            if (StartDateRangeFrom.HasValue)
                queryParameters.Add("startDateRangeFrom", StartDateRangeFrom.Value.ToString("u"));

            if (StartDateRangeTo.HasValue)
                queryParameters.Add("startDateRangeTo", StartDateRangeTo.Value.ToString("u"));

            if (Alert.HasValue)
                queryParameters.Add("alert", Alert.Value.ToString());

            if (ApprenticeConfirmationStatus.HasValue)
                queryParameters.Add("apprenticeConfirmationStatus", ApprenticeConfirmationStatus.Value.ToString());
            if (DeliveryModel.HasValue)
                queryParameters.Add("deliveryModel", DeliveryModel.Value.ToString());

            return QueryHelpers.AddQueryString($"/provider/{ProviderId}/apprentices", queryParameters);
        }
    }
}