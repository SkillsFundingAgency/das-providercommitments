using SFA.DAS.ProviderCommitments.Web.ModelBinding;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class IndexRequest : IAuthorizationContextModel
    {
        private int _pageNumber = 1;
        public long ProviderId { get; set; }

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SearchTerm { get; set; }
        public string SelectedEmployer { get; set; }
        public string SelectedCourse { get; set; }
        public ApprenticeshipStatus? SelectedStatus { get; set; }
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
        public bool FromSearch { get; set; }
        public Alerts? SelectedAlert { get; set; }
        public ConfirmationStatus? SelectedApprenticeConfirmation { get; set; }
        public DeliveryModel? SelectedDeliveryModel { get; set; }
    }   
}
