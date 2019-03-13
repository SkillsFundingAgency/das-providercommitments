using System;
using SFA.DAS.ProviderCommitments.ModelBinding.Models;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class AddDraftApprenticeshipRequest
    {
        public Guid ReservationId { get; set; }
        public UnhashedAccount Account { get; set; }
        public UnhashedAccountLegalEntity AccountLegalEntity { get; set; }
        public string StartMonthYear { get; set; }
        public string CourseCode { get; set; }
    }
}