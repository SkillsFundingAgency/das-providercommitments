﻿using System;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices
{
    public class GetReviewApprenticeshipUpdatesRequest : IGetApiRequest
    {
        public long ProviderId { get; }
        public long ApprenticeshipId { get; set; }

        public GetReviewApprenticeshipUpdatesRequest(long providerId, long apprenticeshipId)
        {
            ProviderId = providerId;
            ApprenticeshipId = apprenticeshipId;
        }

        public string GetUrl => $"provider/{ProviderId}/apprentices/{ApprenticeshipId}/changes/review";
    }

    public class GetReviewApprenticeshipUpdatesResponse
    {
        public bool IsValidCourseCode { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public ApprenticeshipDetails OriginalApprenticeship { get; set; }
        public ApprenticeshipDetails ApprenticeshipUpdates { get; set; }
    }

    public class ApprenticeshipDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Uln { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Version { get; set; }
        public string Option { get; set; }
        public DeliveryModel? DeliveryModel { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public int? EmploymentPrice { get; set; }
        public Decimal? Cost { get; set; }
    }
}
