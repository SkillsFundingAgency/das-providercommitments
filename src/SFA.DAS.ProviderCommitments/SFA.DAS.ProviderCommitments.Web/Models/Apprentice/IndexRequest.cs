using System;
using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class IndexRequest : IAuthorizationContextModel
    {
        public long ProviderId { get; set; }
        public int PageNumber { get; set; }
		public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SearchTerm { get; set; }
        public string SelectedEmployer { get; set; }
        public string SelectedCourse { get; set; }
        public ApprenticeshipStatus? SelectedStatus { get; set; }
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
    }
}
