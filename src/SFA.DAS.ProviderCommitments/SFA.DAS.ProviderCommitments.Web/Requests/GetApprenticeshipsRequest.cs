using System;

namespace SFA.DAS.ProviderCommitments.Web.Requests
{
    public class GetApprenticeshipsRequest
    {
        public long ProviderId { get; set; }
        public int PageNumber { get; set; }
        public int PageItemCount { get; set; }
        public string SearchTerm { get; set; }
        public string SelectedEmployer { get; set; }
        public string SelectedCourse { get; set; }
        public string SelectedStatus { get; set; }
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
    }
}
